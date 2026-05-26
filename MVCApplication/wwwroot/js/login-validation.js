// login-validation.js - Validation for the login form + client-side lockout UX

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('loginForm');

    if (!form) return; // Exit if form doesn't exist on this page

    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');
    const submitButton = form.querySelector('button[type="submit"], input[type="submit"]');
    const lockoutMessageId = 'lockoutMessage';

    // Ensure lockout message element exists (insert after form top)
    function ensureLockoutElement() {
        let el = document.getElementById(lockoutMessageId);
        if (!el) {
            el = document.createElement('div');
            el.id = lockoutMessageId;
            el.className = 'alert alert-danger';
            el.style.display = 'none';
            el.setAttribute('role', 'alert');
            form.parentNode.insertBefore(el, form);
        }
        return el;
    }

    const lockoutEl = ensureLockoutElement();

    // Helper to compute identifier (per-email). If email empty, use 'global'.
    function currentIdentifier() {
        const e = (emailInput && emailInput.value) ? emailInput.value.trim().toLowerCase() : '';
        return e || 'global';
    }

    // Update UI based on lockout state
    function updateLockoutUI() {
        const id = currentIdentifier();
        const scope = 'login';
        const state = ValidationHelpers.LockoutManager.getState(scope, id);

        if (state.locked) {
            // Disable form inputs
            if (submitButton) submitButton.disabled = true;
            [emailInput, passwordInput].forEach(i => { if (i) i.disabled = true; });

            lockoutEl.style.display = 'block';
            lockoutEl.textContent = `Too many failed attempts. Try again in ${ValidationHelpers.LockoutManager.formatMs(state.remainingMs)}.`;
            // Countdown
            startCountdown(state.remainingMs, remaining => {
                if (remaining <= 0) {
                    ValidationHelpers.LockoutManager.resetAttempts(scope, id);
                    updateLockoutUI();
                } else {
                    lockoutEl.textContent = `Too many failed attempts. Try again in ${ValidationHelpers.LockoutManager.formatMs(remaining)}.`;
                }
            });
        } else {
            // Enable inputs
            if (submitButton) submitButton.disabled = false;
            [emailInput, passwordInput].forEach(i => { if (i) i.disabled = false; });

            lockoutEl.style.display = 'none';
            lockoutEl.textContent = '';
        }
    }

    let countdownTimer = null;
    function startCountdown(durationMs, onTick) {
        if (countdownTimer) {
            clearInterval(countdownTimer);
            countdownTimer = null;
        }
        let remaining = durationMs;
        if (remaining <= 0) {
            if (onTick) onTick(0);
            return;
        }
        if (onTick) onTick(remaining);
        countdownTimer = setInterval(() => {
            remaining = remaining - 1000;
            if (remaining <= 0) {
                clearInterval(countdownTimer);
                countdownTimer = null;
                if (onTick) onTick(0);
            } else {
                if (onTick) onTick(remaining);
            }
        }, 1000);
    }

    // Validation functions
    function validateEmail() {
        const email = emailInput.value.trim();
        const errorElement = document.getElementById('emailError');

        const scriptCheck = ValidationHelpers.validateNoScript(email, 'Email');
        if (!scriptCheck.valid) {
            ValidationHelpers.showError(emailInput, errorElement, scriptCheck.message);
            return false;
        }

        const requiredCheck = ValidationHelpers.validateRequired(email, 'Email');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(emailInput, errorElement, requiredCheck.message);
            return false;
        }

        if (!ValidationHelpers.validateEmail(email)) {
            ValidationHelpers.showError(emailInput, errorElement, 'Please enter a valid email address');
            return false;
        }

        ValidationHelpers.clearError(emailInput, errorElement);
        return true;
    }

    function validatePassword() {
        const password = passwordInput.value;
        const errorElement = document.getElementById('passwordError');

        const scriptCheck = ValidationHelpers.validateNoScript(password, 'Password');
        if (!scriptCheck.valid) {
            ValidationHelpers.showError(passwordInput, errorElement, scriptCheck.message);
            return false;
        }

        const requiredCheck = ValidationHelpers.validateRequired(password, 'Password');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(passwordInput, errorElement, requiredCheck.message);
            return false;
        }

        ValidationHelpers.clearError(passwordInput, errorElement);
        return true;
    }

    // Real-time validation on blur
    emailInput.addEventListener('blur', function() {
        validateEmail();
        updateLockoutUI(); // identifier may have changed
    });
    passwordInput.addEventListener('blur', validatePassword);

    // Form submission validation and fetch-based POST to detect server response
    form.addEventListener('submit', function(e) {
        e.preventDefault();

        const id = currentIdentifier();
        const scope = 'login';

        // If locked, show UI and stop
        if (ValidationHelpers.LockoutManager.isLocked(scope, id)) {
            updateLockoutUI();
            return;
        }

        // Validate fields client-side
        const isEmailValid = validateEmail();
        const isPasswordValid = validatePassword();

        if (!(isEmailValid && isPasswordValid)) {
            const errorDiv = document.getElementById('validationErrors');
            if (errorDiv) {
                errorDiv.textContent = 'Please fix the errors above before submitting';
                errorDiv.style.display = 'block';
                window.scrollTo({ top: 0, behavior: 'smooth' });
            }
            return;
        }

        // Submit via fetch so we can detect a failed authentication (non-redirect -> failure)
        const action = form.action || window.location.href;
        const formData = new FormData(form);

        // NOTE: allow same-origin credentials and follow redirects so cookies and redirect info behave like a normal navigation.
        fetch(action, {
            method: 'POST',
            body: formData,
            credentials: 'same-origin',
            redirect: 'follow'
        }).then(async response => {
            // If the fetch followed a redirect, response.redirected will be true and response.url is final location.
            if (response.redirected) {
                // navigate the top-level window to the final URL
                window.location.href = response.url;
                return;
            }

            // If no redirect occurred, treat as failed login (server returned login page or 200 with errors)
            let message = 'Login failed. Please check your credentials.';
            try {
                const text = await response.text();
                const parser = new DOMParser();
                const doc = parser.parseFromString(text, 'text/html');
                const serverErr = doc.getElementById('validationErrors') || doc.querySelector('.validation-summary-errors');
                if (serverErr && serverErr.textContent.trim()) {
                    message = serverErr.textContent.trim();
                }
            } catch {
                // ignore
            }

            // Record failure and update lockout UI
            const result = ValidationHelpers.LockoutManager.recordFailure(scope, id);
            if (result.locked) {
                updateLockoutUI();
            } else {
                // Show inline error near top
                const errorDiv = document.getElementById('validationErrors');
                if (errorDiv) {
                    errorDiv.textContent = message;
                    errorDiv.style.display = 'block';
                    window.scrollTo({ top: 0, behavior: 'smooth' });
                } else {
                    lockoutEl.style.display = 'block';
                    lockoutEl.textContent = message;
                }
            }
        }).catch(err => {
            // Network or unexpected error - do not count as auth failure
            const errorDiv = document.getElementById('validationErrors');
            if (errorDiv) {
                errorDiv.textContent = 'An error occurred while connecting to server. Please try again.';
                errorDiv.style.display = 'block';
            }
            console.error('Login submit error:', err);
        });
    });

    // Initial UI update (in case user is already locked out)
    updateLockoutUI();
});
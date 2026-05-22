// validation-helpers.js - Reusable validation utility functions

const ValidationHelpers = {
    // Email validation
    validateEmail: function(email) {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    },

    // Password strength validation
    validatePassword: function(password, requirements = {}) {
        const defaults = {
            minLength: 6,
            requireUppercase: true,
            requireLowercase: true,
            requireNumber: true,
            requireSpecial: false
        };
        
        const rules = { ...defaults, ...requirements };
        
        if (password.length < rules.minLength) {
            return { valid: false, message: `Password must be at least ${rules.minLength} characters` };
        }
        
        if (rules.requireUppercase && !/[A-Z]/.test(password)) {
            return { valid: false, message: 'Password must contain at least one uppercase letter' };
        }
        
        if (rules.requireLowercase && !/[a-z]/.test(password)) {
            return { valid: false, message: 'Password must contain at least one lowercase letter' };
        }
        
        if (rules.requireNumber && !/[0-9]/.test(password)) {
            return { valid: false, message: 'Password must contain at least one number' };
        }
        
        if (rules.requireSpecial && !/[!@#$%^&*(),.?":{}|<>]/.test(password)) {
            return { valid: false, message: 'Password must contain at least one special character' };
        }
        
        return { valid: true, message: '' };
    },

    // Username validation
    validateUsername: function(username, minLength = 3) {
        if (username.length < minLength) {
            return { valid: false, message: `Username must be at least ${minLength} characters` };
        }
        
        if (!/^[a-zA-Z0-9_]+$/.test(username)) {
            return { valid: false, message: 'Username can only contain letters, numbers, and underscores' };
        }
        
        return { valid: true, message: '' };
    },

    // Required field validation
    validateRequired: function(value, fieldName = 'This field') {
        if (!value || value.trim() === '') {
            return { valid: false, message: `${fieldName} is required` };
        }
        return { valid: true, message: '' };
    },

    // Min/Max length validation
    validateLength: function(value, min, max, fieldName = 'This field') {
        if (min && value.length < min) {
            return { valid: false, message: `${fieldName} must be at least ${min} characters` };
        }
        
        if (max && value.length > max) {
            return { valid: false, message: `${fieldName} cannot exceed ${max} characters` };
        }
        
        return { valid: true, message: '' };
    },

    // Date validation
    validateDate: function(dateString, options = {}) {
        const date = new Date(dateString);
        
        if (isNaN(date.getTime())) {
            return { valid: false, message: 'Please enter a valid date' };
        }
        
        if (options.futureOnly && date < new Date()) {
            return { valid: false, message: 'Date must be in the future' };
        }
        
        if (options.pastOnly && date > new Date()) {
            return { valid: false, message: 'Date must be in the past' };
        }
        
        return { valid: true, message: '' };
    },

    // Reject obvious script/HTML patterns in input (client-side defensive check)
    validateNoScript: function(value, fieldName = 'This field') {
        if (!value || value.trim() === '') return { valid: true, message: '' };

        // Common patterns: <script>, </script>, javascript: URIs, inline event handlers (on...)
        const patterns = [
            /<\s*script\b/i,
            /<\s*\/\s*script\s*>/i,
            /javascript\s*:/i,
            /on\w+\s*=/i
        ];

        for (const p of patterns) {
            if (p.test(value)) {
                return { valid: false, message: `Invalid input for ${fieldName}: scripts or HTML are not allowed` };
            }
        }

        return { valid: true, message: '' };
    },

    // Show error on input - UPDATED (guard errorElement to avoid runtime exceptions)
    showError: function(inputElement, errorElement, message) {
        if (inputElement && !inputElement.classList.contains('invalid')) {
            inputElement.classList.add('invalid');
        }
        if (inputElement && inputElement.classList.contains('valid')) {
            inputElement.classList.remove('valid');
        }

        if (errorElement) {
            errorElement.textContent = message || '';
            errorElement.style.visibility = message ? 'visible' : 'hidden';
        }

        // Provide accessible state
        if (inputElement) {
            inputElement.setAttribute('aria-invalid', 'true');
        }
    },

    // Clear error from input - UPDATED (guard errorElement)
    clearError: function(inputElement, errorElement) {
        if (inputElement) {
            inputElement.classList.remove('invalid');
            if (!inputElement.classList.contains('valid')) {
                inputElement.classList.add('valid');
            }
            inputElement.removeAttribute('aria-invalid');
        }

        if (errorElement) {
            errorElement.textContent = '';
            errorElement.style.visibility = 'hidden';
        }
    },

    // Clear all errors in a form
    clearAllErrors: function(formElement) {
        const errorElements = formElement.querySelectorAll('.error-message');
        const inputElements = formElement.querySelectorAll('input, textarea, select');
        
        errorElements.forEach(el => {
            el.textContent = '';
            el.style.visibility = 'hidden';
        });
        
        inputElements.forEach(el => {
            el.classList.remove('invalid', 'valid');
            el.removeAttribute('aria-invalid');
        });
    }
};

// LockoutManager: simple client-side attempt tracker using localStorage
// IMPORTANT: Client-side only — enforce lockout on server for real security.
ValidationHelpers.LockoutManager = (function() {
    const STORAGE_PREFIX = 'app_lockout';
    const defaults = {
        maxAttempts: 5,
        lockoutDurationMs: 15 * 60 * 1000 // 15 minutes
    };

    function storageKey(scope, identifier) {
        const id = identifier ? identifier.toString().toLowerCase() : 'global';
        return `${STORAGE_PREFIX}:${scope}:${id}`;
    }

    function now() {
        return Date.now();
    }

    function readState(scope, identifier) {
        try {
            const raw = localStorage.getItem(storageKey(scope, identifier));
            if (!raw) return { attempts: 0, lockedUntil: 0 };
            const parsed = JSON.parse(raw);
            return {
                attempts: parsed.attempts || 0,
                lockedUntil: parsed.lockedUntil || 0
            };
        } catch {
            return { attempts: 0, lockedUntil: 0 };
        }
    }

    function writeState(scope, identifier, state) {
        try {
            localStorage.setItem(storageKey(scope, identifier), JSON.stringify(state));
        } catch {
            // ignore storage errors
        }
    }

    function clearState(scope, identifier) {
        try {
            localStorage.removeItem(storageKey(scope, identifier));
        } catch {
            // ignore
        }
    }

    function isLocked(scope, identifier) {
        const s = readState(scope, identifier);
        return s.lockedUntil && s.lockedUntil > now();
    }

    function remainingMs(scope, identifier) {
        const s = readState(scope, identifier);
        return Math.max(0, (s.lockedUntil || 0) - now());
    }

    function recordFailure(scope, identifier, options = {}) {
        const cfg = { ...defaults, ...(options || {}) };
        const s = readState(scope, identifier);
        s.attempts = (s.attempts || 0) + 1;
        if (s.attempts >= cfg.maxAttempts) {
            s.lockedUntil = now() + cfg.lockoutDurationMs;
            s.attempts = 0; // reset attempts after locking
        }
        writeState(scope, identifier, s);
        return { locked: !!s.lockedUntil && s.lockedUntil > now(), attempts: s.attempts, lockedUntil: s.lockedUntil || 0 };
    }

    function resetAttempts(scope, identifier) {
        clearState(scope, identifier);
    }

    function getState(scope, identifier) {
        const s = readState(scope, identifier);
        return {
            attempts: s.attempts || 0,
            lockedUntil: s.lockedUntil || 0,
            locked: s.lockedUntil && s.lockedUntil > now(),
            remainingMs: Math.max(0, (s.lockedUntil || 0) - now())
        };
    }

    function formatMs(ms) {
        const total = Math.max(0, Math.floor(ms / 1000));
        const minutes = Math.floor(total / 60);
        const seconds = total % 60;
        return `${minutes}:${seconds.toString().padStart(2, '0')}`;
    }

    return {
        defaults: { ...defaults },
        isLocked: isLocked,
        remainingMs: remainingMs,
        recordFailure: recordFailure,
        resetAttempts: resetAttempts,
        getState: getState,
        storageKey: storageKey,
        formatMs: formatMs
    };
})();

// Make it available globally
window.ValidationHelpers = ValidationHelpers;
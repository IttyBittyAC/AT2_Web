// login-validation.js - Validation for the login form

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('loginForm');

    if (!form) return; // Exit if form doesn't exist on this page

    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');

    // Validation functions
    function validateEmail() {
        const email = emailInput.value.trim();
        const errorElement = document.getElementById('emailError');

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

        const requiredCheck = ValidationHelpers.validateRequired(password, 'Password');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(passwordInput, errorElement, requiredCheck.message);
            return false;
        }

        ValidationHelpers.clearError(passwordInput, errorElement);
        return true;
    }

    // Real-time validation on blur
    emailInput.addEventListener('blur', validateEmail);
    passwordInput.addEventListener('blur', validatePassword);

    // Form submission validation
    form.addEventListener('submit', function(e) {
        e.preventDefault();

        // Validate all fields
        const isEmailValid = validateEmail();
        const isPasswordValid = validatePassword();

        // Check if all validations passed
        if (isEmailValid && isPasswordValid) {
            // All validations passed, submit the form
            form.submit();
        } else {
            // Show error message at top
            const errorDiv = document.getElementById('validationErrors');
            if (errorDiv) {
                errorDiv.textContent = 'Please fix the errors above before submitting';
                errorDiv.style.display = 'block';

                // Scroll to top to show errors
                window.scrollTo({ top: 0, behavior: 'smooth' });
            }
        }
    });
});
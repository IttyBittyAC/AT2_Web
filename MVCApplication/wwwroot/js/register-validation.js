// register-validation.js - Validation for the registration form

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('registerForm');

    if (!form) return; // Exit if form doesn't exist on this page

    const fullnameInput = document.getElementById('fullname');
    const usernameInput = document.getElementById('username');
    const emailInput = document.getElementById('email');
    const passwordInput = document.getElementById('password');
    const confirmPasswordInput = document.getElementById('confirmPassword');

    // Validation functions
    function validateFullName() {
        const fullname = fullnameInput.value.trim();
        const errorElement = document.getElementById('fullnameError');

        const requiredCheck = ValidationHelpers.validateRequired(fullname, 'Full name');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(fullnameInput, errorElement, requiredCheck.message);
            return false;
        }

        const lengthCheck = ValidationHelpers.validateLength(fullname, 2, null, 'Full name');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(fullnameInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(fullnameInput, errorElement);
        return true;
    }

    function validateUsername() {
        const username = usernameInput.value.trim();
        const errorElement = document.getElementById('usernameError');

        const requiredCheck = ValidationHelpers.validateRequired(username, 'Username');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(usernameInput, errorElement, requiredCheck.message);
            return false;
        }

        const usernameCheck = ValidationHelpers.validateUsername(username, 3);
        if (!usernameCheck.valid) {
            ValidationHelpers.showError(usernameInput, errorElement, usernameCheck.message);
            return false;
        }

        ValidationHelpers.clearError(usernameInput, errorElement);
        return true;
    }

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

        const passwordCheck = ValidationHelpers.validatePassword(password);
        if (!passwordCheck.valid) {
            ValidationHelpers.showError(passwordInput, errorElement, passwordCheck.message);
            return false;
        }

        ValidationHelpers.clearError(passwordInput, errorElement);
        return true;
    }

    function validateConfirmPassword() {
        const password = passwordInput.value;
        const confirmPassword = confirmPasswordInput.value;
        const errorElement = document.getElementById('confirmPasswordError');

        const requiredCheck = ValidationHelpers.validateRequired(confirmPassword, 'Password confirmation');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(confirmPasswordInput, errorElement, requiredCheck.message);
            return false;
        }

        if (password !== confirmPassword) {
            ValidationHelpers.showError(confirmPasswordInput, errorElement, 'Passwords do not match');
            return false;
        }

        ValidationHelpers.clearError(confirmPasswordInput, errorElement);
        return true;
    }

    // Real-time validation on blur
    fullnameInput.addEventListener('blur', validateFullName);
    usernameInput.addEventListener('blur', validateUsername);
    emailInput.addEventListener('blur', validateEmail);
    passwordInput.addEventListener('blur', validatePassword);
    confirmPasswordInput.addEventListener('blur', validateConfirmPassword);

    // Validate confirm password when password changes
    passwordInput.addEventListener('input', function() {
        if (confirmPasswordInput.value !== '') {
            validateConfirmPassword();
        }
    });

    // Form submission validation
    form.addEventListener('submit', function(e) {
        e.preventDefault();

        // Validate all fields
        const isFullNameValid = validateFullName();
        const isUsernameValid = validateUsername();
        const isEmailValid = validateEmail();
        const isPasswordValid = validatePassword();
        const isConfirmPasswordValid = validateConfirmPassword();

        // Check if all validations passed
        if (isFullNameValid && isUsernameValid && isEmailValid && isPasswordValid && isConfirmPasswordValid) {
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
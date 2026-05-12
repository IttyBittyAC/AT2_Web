document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('profileForm');

    if (!form) return;

    const usernameInput = document.getElementById('Username');
    const fullNameInput = document.getElementById('FullName');
    const emailInput = document.getElementById('Email');
    const validationErrors = document.getElementById('validationErrors');

    function hideSummary() {
        if (validationErrors) {
            validationErrors.style.display = 'none';
        }
    }

    function showSummary() {
        if (validationErrors) {
            validationErrors.style.display = 'block';
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    }

    function validateUsername() {
        const value = usernameInput.value.trim();
        const errorElement = document.getElementById('usernameError');

        const requiredCheck = ValidationHelpers.validateRequired(value, 'Username');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(usernameInput, errorElement, requiredCheck.message);
            return false;
        }

        const usernameCheck = ValidationHelpers.validateUsername(value, 3);
        if (!usernameCheck.valid) {
            ValidationHelpers.showError(usernameInput, errorElement, usernameCheck.message);
            return false;
        }

        ValidationHelpers.clearError(usernameInput, errorElement);
        return true;
    }

    function validateFullName() {
        const value = fullNameInput.value.trim();
        const errorElement = document.getElementById('fullNameError');

        const requiredCheck = ValidationHelpers.validateRequired(value, 'Full name');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(fullNameInput, errorElement, requiredCheck.message);
            return false;
        }

        const lengthCheck = ValidationHelpers.validateLength(value, 2, 100, 'Full name');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(fullNameInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(fullNameInput, errorElement);
        return true;
    }

    function validateEmail() {
        const value = emailInput.value.trim();
        const errorElement = document.getElementById('emailError');

        const requiredCheck = ValidationHelpers.validateRequired(value, 'Email');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(emailInput, errorElement, requiredCheck.message);
            return false;
        }

        if (!ValidationHelpers.validateEmail(value)) {
            ValidationHelpers.showError(emailInput, errorElement, 'Please enter a valid email address');
            return false;
        }

        ValidationHelpers.clearError(emailInput, errorElement);
        return true;
    }

    usernameInput.addEventListener('blur', function () {
        validateUsername();
        hideSummary();
    });

    fullNameInput.addEventListener('blur', function () {
        validateFullName();
        hideSummary();
    });

    emailInput.addEventListener('blur', function () {
        validateEmail();
        hideSummary();
    });

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const isUsernameValid = validateUsername();
        const isFullNameValid = validateFullName();
        const isEmailValid = validateEmail();

        if (isUsernameValid && isFullNameValid && isEmailValid) {
            hideSummary();
            form.submit();
            return;
        }

        showSummary();
    });
});
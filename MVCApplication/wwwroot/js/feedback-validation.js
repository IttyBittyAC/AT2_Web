// feedback-validation.js - Validation for the feedback form

document.addEventListener('DOMContentLoaded', function() {
    const form = document.getElementById('feedbackForm');

    if (!form) return; // Exit if form doesn't exist on this page

    const typeInput = document.getElementById('Type');
    const headingInput = document.getElementById('Heading');
    const messageInput = document.getElementById('Message');
    const fullnameInput = document.getElementById('FullName');
    const emailInput = document.getElementById('Email');

    // Validation functions
    function validateType() {
        const type = typeInput.value;
        const errorElement = document.getElementById('typeError');

        const requiredCheck = ValidationHelpers.validateRequired(type, 'Feedback type');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(typeInput, errorElement, requiredCheck.message);
            return false;
        }

        ValidationHelpers.clearError(typeInput, errorElement);
        return true;
    }

    function validateHeading() {
        const heading = headingInput.value.trim();
        const errorElement = document.getElementById('headingError');

        const requiredCheck = ValidationHelpers.validateRequired(heading, 'Subject');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(headingInput, errorElement, requiredCheck.message);
            return false;
        }

        const lengthCheck = ValidationHelpers.validateLength(heading, 5, 200, 'Subject');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(headingInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(headingInput, errorElement);
        return true;
    }

    function validateMessage() {
        const message = messageInput.value.trim();
        const errorElement = document.getElementById('messageError');

        const requiredCheck = ValidationHelpers.validateRequired(message, 'Message');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(messageInput, errorElement, requiredCheck.message);
            return false;
        }

        const lengthCheck = ValidationHelpers.validateLength(message, 10, 2000, 'Message');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(messageInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(messageInput, errorElement);
        return true;
    }

    function validateFullName() {
        const fullname = fullnameInput.value.trim();
        const errorElement = document.getElementById('fullnameError');

        const requiredCheck = ValidationHelpers.validateRequired(fullname, 'Full name');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(fullnameInput, errorElement, requiredCheck.message);
            return false;
        }

        const lengthCheck = ValidationHelpers.validateLength(fullname, 2, 100, 'Full name');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(fullnameInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(fullnameInput, errorElement);
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

    // Real-time validation on blur
    typeInput.addEventListener('blur', validateType);
    headingInput.addEventListener('blur', validateHeading);
    messageInput.addEventListener('blur', validateMessage);
    fullnameInput.addEventListener('blur', validateFullName);
    emailInput.addEventListener('blur', validateEmail);

    // Form submission validation
    form.addEventListener('submit', function(e) {
        e.preventDefault();

        // Validate all fields
        const isTypeValid = validateType();
        const isHeadingValid = validateHeading();
        const isMessageValid = validateMessage();
        const isFullNameValid = validateFullName();
        const isEmailValid = validateEmail();

        // Check if all validations passed
        if (isTypeValid && isHeadingValid && isMessageValid && isFullNameValid && isEmailValid) {
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
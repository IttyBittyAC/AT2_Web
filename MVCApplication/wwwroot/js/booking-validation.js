// booking-validation.js - Validation for the booking form

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('bookingForm');

    if (!form) return;

    const fullNameInput = document.getElementById('FullName');
    const emailInput = document.getElementById('Email');
    const bookingDateInput = document.getElementById('BookingDate');

    function validateFullName() {
        const fullName = fullNameInput.value.trim();
        const errorElement = document.getElementById('fullNameError');

        const requiredCheck = ValidationHelpers.validateRequired(fullName, 'Full name');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(fullNameInput, errorElement, requiredCheck.message);
            return false;
        }

        const lengthCheck = ValidationHelpers.validateLength(fullName, 2, 100, 'Full name');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(fullNameInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(fullNameInput, errorElement);
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

    function validateBookingDate() {
        const bookingDate = bookingDateInput.value;
        const errorElement = document.getElementById('bookingDateError');

        const requiredCheck = ValidationHelpers.validateRequired(bookingDate, 'Preferred date');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(bookingDateInput, errorElement, requiredCheck.message);
            return false;
        }

        const dateCheck = ValidationHelpers.validateDate(bookingDate, { futureOnly: true });
        if (!dateCheck.valid) {
            ValidationHelpers.showError(bookingDateInput, errorElement, dateCheck.message);
            return false;
        }

        ValidationHelpers.clearError(bookingDateInput, errorElement);
        return true;
    }

    fullNameInput.addEventListener('blur', validateFullName);
    emailInput.addEventListener('blur', validateEmail);
    bookingDateInput.addEventListener('blur', validateBookingDate);

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const isFullNameValid = validateFullName();
        const isEmailValid = validateEmail();
        const isBookingDateValid = validateBookingDate();

        if (isFullNameValid && isEmailValid && isBookingDateValid) {
            form.submit();
            return;
        }

        const errorDiv = document.getElementById('validationErrors');
        if (errorDiv) {
            errorDiv.textContent = 'Please fix the errors above before submitting';
            errorDiv.style.display = 'block';
            window.scrollTo({ top: 0, behavior: 'smooth' });
        }
    });
});
// create-event-validation.js - Validation for the create event form

document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('createEventForm');

    if (!form) return;

    const titleInput = document.getElementById('Title');
    const descriptionInput = document.getElementById('Description');
    const eventDateInput = document.getElementById('EventDate');
    const locationInput = document.getElementById('Location');
    const counter = document.getElementById('descriptionCounter');

    function updateCounter() {
        const length = descriptionInput.value.length;
        counter.textContent = length + ' / 2000 characters';

        if (length > 1900) {
            counter.classList.add('danger');
            counter.classList.remove('warning');
        } else if (length > 1700) {
            counter.classList.add('warning');
            counter.classList.remove('danger');
        } else {
            counter.classList.remove('warning', 'danger');
        }
    }

    function validateTitle() {
        const title = titleInput.value.trim();
        const errorElement = document.getElementById('titleError');

        const requiredCheck = ValidationHelpers.validateRequired(title, 'Event title');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(titleInput, errorElement, requiredCheck.message);
            return false;
        }

        const lengthCheck = ValidationHelpers.validateLength(title, 5, 200, 'Event title');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(titleInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(titleInput, errorElement);
        return true;
    }

    function validateDescription() {
        const description = descriptionInput.value.trim();
        const errorElement = document.getElementById('descriptionError');

        const lengthCheck = ValidationHelpers.validateLength(description, 0, 2000, 'Event description');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(descriptionInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(descriptionInput, errorElement);
        return true;
    }

    function validateEventDate() {
        const eventDate = eventDateInput.value;
        const errorElement = document.getElementById('eventDateError');

        const requiredCheck = ValidationHelpers.validateRequired(eventDate, 'Event date and time');
        if (!requiredCheck.valid) {
            ValidationHelpers.showError(eventDateInput, errorElement, requiredCheck.message);
            return false;
        }

        const dateCheck = ValidationHelpers.validateDate(eventDate, { futureOnly: true });
        if (!dateCheck.valid) {
            ValidationHelpers.showError(eventDateInput, errorElement, dateCheck.message);
            return false;
        }

        ValidationHelpers.clearError(eventDateInput, errorElement);
        return true;
    }

    function validateLocation() {
        const location = locationInput.value.trim();
        const errorElement = document.getElementById('locationError');

        const lengthCheck = ValidationHelpers.validateLength(location, 0, 200, 'Location');
        if (!lengthCheck.valid) {
            ValidationHelpers.showError(locationInput, errorElement, lengthCheck.message);
            return false;
        }

        ValidationHelpers.clearError(locationInput, errorElement);
        return true;
    }

    titleInput.addEventListener('blur', validateTitle);
    descriptionInput.addEventListener('blur', validateDescription);
    eventDateInput.addEventListener('blur', validateEventDate);
    locationInput.addEventListener('blur', validateLocation);
    descriptionInput.addEventListener('input', updateCounter);

    updateCounter();

    form.addEventListener('submit', function (e) {
        e.preventDefault();

        const isTitleValid = validateTitle();
        const isDescriptionValid = validateDescription();
        const isEventDateValid = validateEventDate();
        const isLocationValid = validateLocation();

        if (isTitleValid && isDescriptionValid && isEventDateValid && isLocationValid) {
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
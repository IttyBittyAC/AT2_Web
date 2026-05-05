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

    // Show error on input - UPDATED
    showError: function(inputElement, errorElement, message) {
        inputElement.classList.add('invalid');
        inputElement.classList.remove('valid');
        errorElement.textContent = message;
        errorElement.style.visibility = 'visible';  // Changed from display
    },

    // Clear error from input - UPDATED
    clearError: function(inputElement, errorElement) {
        inputElement.classList.remove('invalid');
        inputElement.classList.add('valid');
        errorElement.textContent = '';
        errorElement.style.visibility = 'hidden';  // Changed from display
    },

    // Clear all errors in a form
    clearAllErrors: function(formElement) {
        const errorElements = formElement.querySelectorAll('.error-message');
        const inputElements = formElement.querySelectorAll('input, textarea, select');
        
        errorElements.forEach(el => {
            el.textContent = '';
            el.style.visibility = 'hidden';  // Changed from display
        });
        
        inputElements.forEach(el => {
            el.classList.remove('invalid', 'valid');
        });
    }
};

// Make it available globally
window.ValidationHelpers = ValidationHelpers;
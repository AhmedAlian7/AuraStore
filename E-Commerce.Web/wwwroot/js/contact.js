$(document).ready(function () {
    // Character counter for message textarea
    $('#Message').on('input', function () {
        const current = $(this).val().length;
        $('#charCount').text(current);

        if (current > 2000) {
            $(this).addClass('is-invalid');
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    // Real-time validation
    $('input, select, textarea').on('blur', function () {
        validateField($(this));
    });

    // Form submission
    $('#contactForm').on('submit', function (e) {
        e.preventDefault();

        if (validateForm()) {
            submitForm();
        }
    });

    function validateField($field) {
        const fieldName = $field.attr('name');
        const value = $field.val().trim();
        let isValid = true;
        let errorMessage = '';

        // Remove existing validation classes
        $field.removeClass('is-valid is-invalid');
        $field.siblings('.text-danger').text('');

        // Name validation
        if (fieldName === 'Name') {
            if (!value) {
                isValid = false;
                errorMessage = 'Name is required';
            } else if (value.length > 100) {
                isValid = false;
                errorMessage = 'Name cannot exceed 100 characters';
            }
        }

        // Email validation
        if (fieldName === 'Email') {
            const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
            if (!value) {
                isValid = false;
                errorMessage = 'Email is required';
            } else if (!emailRegex.test(value)) {
                isValid = false;
                errorMessage = 'Please enter a valid email address';
            } else if (value.length > 254) {
                isValid = false;
                errorMessage = 'Email cannot exceed 254 characters';
            }
        }

        // Inquiry Type validation
        if (fieldName === 'InquiryType') {
            if (!value) {
                isValid = false;
                errorMessage = 'Please select an inquiry type';
            }
        }

        // Subject validation
        if (fieldName === 'Subject') {
            if (!value) {
                isValid = false;
                errorMessage = 'Subject is required';
            } else if (value.length > 200) {
                isValid = false;
                errorMessage = 'Subject cannot exceed 200 characters';
            }
        }

        // Message validation
        if (fieldName === 'Message') {
            if (!value) {
                isValid = false;
                errorMessage = 'Message is required';
            } else if (value.length < 10) {
                isValid = false;
                errorMessage = 'Message must be at least 10 characters';
            } else if (value.length > 2000) {
                isValid = false;
                errorMessage = 'Message cannot exceed 2000 characters';
            }
        }

        // Order Number validation (optional field)
        if (fieldName === 'OrderNumber' && value) {
            const orderRegex = /^[A-Za-z0-9\-_]*$/;
            if (!orderRegex.test(value)) {
                isValid = false;
                errorMessage = 'Order number can only contain letters, numbers, hyphens, and underscores';
            } else if (value.length > 50) {
                isValid = false;
                errorMessage = 'Order number cannot exceed 50 characters';
            }
        }

        // Apply validation styling
        if (isValid && value) {
            $field.addClass('is-valid');
        } else if (!isValid) {
            $field.addClass('is-invalid');
            $field.siblings('.text-danger').text(errorMessage);
        }

        return isValid;
    }

    function validateForm() {
        let isFormValid = true;

        // Validate all required fields
        $('#contactForm input[required], #contactForm select[required], #contactForm textarea[required]').each(function () {
            if (!validateField($(this))) {
                isFormValid = false;
            }
        });

        // Validate optional fields that have values
        $('#contactForm input:not([required]), #contactForm select:not([required]), #contactForm textarea:not([required])').each(function () {
            if ($(this).val().trim()) {
                if (!validateField($(this))) {
                    isFormValid = false;
                }
            }
        });

        return isFormValid;
    }

    function submitForm() {
        const $submitBtn = $('#submitBtn');
        const $form = $('#contactForm');

        // Show loading state
        $submitBtn.prop('disabled', true);
        $('.btn-text').hide();
        $('.btn-loading').show();
        $('#loadingOverlay').show();

        // Submit form
        $.ajax({
            url: $form.attr('action'),
            method: 'POST',
            data: $form.serialize(),
            success: function (response) {
                // Hide loading
                hideLoading();

                // Show success message
                Swal.fire({
                    icon: 'success',
                    title: 'Message Sent!',
                    text: 'Thank you for contacting us! We\'ll get back to you within 24 hours.',
                    confirmButtonText: 'OK',
                    confirmButtonColor: '#007bff'
                }).then(() => {
                    // Reset form
                    $form[0].reset();
                    $form.find('.is-valid, .is-invalid').removeClass('is-valid is-invalid');
                    $('#charCount').text('0');
                });
            },
            error: function (xhr) {
                hideLoading();

                if (xhr.status === 400) {
                    // Validation errors - update form with server-side validation
                    const errors = xhr.responseJSON;
                    if (errors && errors.errors) {
                        // Handle model validation errors
                        Object.keys(errors.errors).forEach(function (field) {
                            const $field = $(`[name="${field}"]`);
                            $field.addClass('is-invalid');
                            $field.siblings('.text-danger').text(errors.errors[field][0]);
                        });
                    }
                } else {
                    // Show error message
                    Swal.fire({
                        icon: 'error',
                        title: 'Oops!',
                        text: 'Sorry, we couldn\'t send your message at this time. Please try again later.',
                        confirmButtonText: 'OK',
                        confirmButtonColor: '#dc3545'
                    });
                }
            }
        });
    }

    function hideLoading() {
        $('#submitBtn').prop('disabled', false);
        $('.btn-text').show();
        $('.btn-loading').hide();
        $('#loadingOverlay').hide();
    }

    // Show success message if redirected with success
    if (TempData["ContactSuccess"] != null) {
        <text>
            Swal.fire({
                icon: 'success',
            title: 'Message Sent!',
            text: '@TempData["ContactSuccess"]',
            confirmButtonText: 'OK',
            confirmButtonColor: '#007bff'
                });
        </text>
    }
});
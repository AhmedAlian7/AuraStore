// NotifyMe functionality for out-of-stock products
function notifyWhenAvailable(productId) {
    // Show loading state
    const button = event.target;
    const originalText = button.innerHTML;
    button.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Processing...';
    button.disabled = true;

    $.ajax({
        url: '/Customer/Product/NotifyMe',
        type: 'POST',
        data: { productId: productId },
        success: function(response) {
            if (response.success) {
                // Show success message
                Swal.fire({
                    icon: 'success',
                    title: 'Notification Registered!',
                    text: response.message,
                    confirmButtonText: 'OK'
                });
                
                // Update button to show "Already Registered"
                button.innerHTML = '<i class="fas fa-bell"></i> Already Registered';
                button.classList.remove('btn-primary');
                button.classList.add('btn-secondary');
                button.disabled = true;
            } else {
                // Show error message
                Swal.fire({
                    icon: 'warning',
                    title: 'Notification Failed',
                    text: response.message,
                    confirmButtonText: 'OK'
                });
                
                // Reset button
                button.innerHTML = originalText;
                button.disabled = false;
            }
        },
        error: function() {
            // Show error message
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'An error occurred while processing your request. Please try again.',
                confirmButtonText: 'OK'
            });
            
            // Reset button
            button.innerHTML = originalText;
            button.disabled = false;
        }
    });
}

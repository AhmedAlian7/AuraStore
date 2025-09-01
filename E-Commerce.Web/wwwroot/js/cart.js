$(document).ready(function () {
    // Initialize Add to Cart buttons
    initializeAddToCartButtons();
});

function initializeAddToCartButtons() {
    $('.add-to-cart-btn').on('click', function () {
        const button = $(this);
        const productId = button.data('product-id');
        
        // Disable button while processing
        button.prop('disabled', true);
        var originalText = button.html();
        button.html('<i class="fas fa-spinner fa-spin"></i> Processing...');
        
        $.ajax({
            url: '/Customer/Product/AddToCartAjax',
            type: 'POST',
            data: { productId: productId, quantity: 1 },
            success: function (response) {
                if (response.success) {
                    // Update cart badge if it exists
                    const cartBadge = $('.cart-badge');
                    if (cartBadge.length > 0) {
                        cartBadge.text(response.cartCount).show();
                    }
                    
                    // Show success message
                    Swal.fire({
                        icon: 'success',
                        title: 'Added to Cart!',
                        text: response.message,
                        timer: 2000,
                        showConfirmButton: false
                    });
                } else {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: response.message
                    });
                }
            },
            error: function () {
                Swal.fire({
                    icon: 'error',
                    title: 'Error',
                    text: 'Failed to add product to cart. Please try again.'
                });
            },
            complete: function () {
                // Re-enable button after request completes
                button.prop('disabled', false);
                button.html(originalText);            }
        });
    });
}
function swapMainImage(imageUrl) {
    const mainImage = document.getElementById('mainProductImage');
    mainImage.src = imageUrl;

    // Update active thumbnail
    document.querySelectorAll('.thumbnail').forEach(thumb => {
        thumb.classList.remove('active');
    });
    event.target.classList.add('active');
}

function toggleDescription() {
    const content = document.getElementById('descriptionContent');
    const button = document.getElementById('toggleDescription');

    if (content.classList.contains('collapsed')) {
        content.classList.remove('collapsed');
        button.textContent = 'Show Less';
    } else {
        content.classList.add('collapsed');
        button.textContent = 'Show More';
    }
}

function showTab(tabName) {
    // Hide all tab contents
    document.querySelectorAll('.tab-content').forEach(tab => {
        tab.classList.remove('active');
    });

    // Remove active class from all headers
    document.querySelectorAll('.tab-header').forEach(header => {
        header.classList.remove('active');
    });

    // Show selected tab
    document.getElementById(tabName).classList.add('active');

    // Add active class to clicked header
    event.target.classList.add('active');
}

function addToCart(productId) {
    const quantity = parseInt(document.getElementById('quantity').value) || 1;
    
    $.ajax({
        url: '/Customer/Product/AddToCartAjax',
        type: 'POST',
        data: { productId: productId, quantity: quantity },
        success: function (response) {
            if (response.success) {
                // Update cart badge in header
                if (typeof updateCartBadge === 'function') {
                    updateCartBadge(response.cartCount);
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
                    text: response.message,
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to add product to cart. Please try again.',
            });
        }
    });
}

function addToWishlist(productId) {
    const button = document.querySelector(`[data-product-id="${productId}"].wishlist-btn`);
    const isActive = button && button.classList.contains('active');
    
    if (isActive) {
        removeFromWishlist(productId, button);
    } else {
        addToWishlistItem(productId, button);
    }
}

function addToWishlistItem(productId, button) {
    $.ajax({
        url: '/Customer/Wishlist/AddToWishlist',
        type: 'POST',
        data: { productId: productId },
        success: function (response) {
            if (response.success) {
                // Update button state
                if (button) {
                    button.classList.add('active');
                    const icon = button.querySelector('i');
                    if (icon) {
                        icon.classList.remove('far');
                        icon.classList.add('fas');
                    }
                    button.textContent = 'Wishlisted';
                    button.title = 'Remove from wishlist';
                }
                
                // Update wishlist badge in header
                if (typeof updateWishlistBadge === 'function') {
                    updateWishlistBadge(response.wishlistCount);
                }
                
                // Show success message
                Swal.fire({
                    icon: 'success',
                    title: 'Added to Wishlist!',
                    text: response.message,
                    timer: 2000,
                    showConfirmButton: false
                });
            } else {
                Swal.fire({
                    icon: 'info',
                    title: 'Already in Wishlist',
                    text: response.message,
                    timer: 2000,
                    showConfirmButton: false
                });
            }
        },
        error: function () {
            Swal.fire({
                icon: 'error',
                title: 'Error',
                text: 'Failed to add product to wishlist. Please try again.',
            });
        }
    });
}

function removeFromWishlist(productId, button) {
    Swal.fire({
        title: 'Remove from Wishlist?',
        text: 'Are you sure you want to remove this item from your wishlist?',
        icon: 'question',
        showCancelButton: true,
        confirmButtonColor: '#d33',
        cancelButtonColor: '#3085d6',
        confirmButtonText: 'Yes, remove it!'
    }).then((result) => {
        if (result.isConfirmed) {
            $.ajax({
                url: '/Customer/Wishlist/RemoveFromWishlist',
                type: 'POST',
                data: { productId: productId },
                success: function (response) {
                    if (response.success) {
                        // Update button state
                        if (button) {
                            button.classList.remove('active');
                            const icon = button.querySelector('i');
                            if (icon) {
                                icon.classList.remove('fas');
                                icon.classList.add('far');
                            }
                            button.textContent = 'Wishlist';
                            button.title = 'Add to wishlist';
                        }
                        
                        // Update wishlist badge in header
                        if (typeof updateWishlistBadge === 'function') {
                            updateWishlistBadge(response.wishlistCount);
                        }
                        
                        // Show success message
                        Swal.fire({
                            icon: 'success',
                            title: 'Removed from Wishlist!',
                            text: response.message,
                            timer: 2000,
                            showConfirmButton: false
                        });
                    } else {
                        Swal.fire({
                            icon: 'error',
                            title: 'Error',
                            text: response.message,
                        });
                    }
                },
                error: function () {
                    Swal.fire({
                        icon: 'error',
                        title: 'Error',
                        text: 'Failed to remove product from wishlist. Please try again.',
                    });
                }
            });
        }
    });
}

function notifyWhenAvailable(productId) {
    // Implement notification functionality
    console.log('Setting notification for product:', productId);
    alert('You will be notified when this product becomes available!');
}

function shareProduct() {
    // Implement share functionality
    if (navigator.share) {
        navigator.share({
            title: document.title,
            url: window.location.href
        });
    } else {
        // Fallback - copy to clipboard
        navigator.clipboard.writeText(window.location.href);
        alert('Product link copied to clipboard!');
    }
}

function likeReview(reviewId) {
    console.log('Liking review:', reviewId);
    alert('Thank you for your feedback!');
}

function replyToReview(reviewId) {
    console.log('Replying to review:', reviewId);
    alert('Reply functionality coming soon!');
}

// Initialize collapsed description if content is long
document.addEventListener('DOMContentLoaded', function () {
    const descriptionContent = document.getElementById('descriptionContent');
    const toggleButton = document.getElementById('toggleDescription');

    if (toggleButton && descriptionContent) {
        descriptionContent.classList.add('collapsed');
    }
});
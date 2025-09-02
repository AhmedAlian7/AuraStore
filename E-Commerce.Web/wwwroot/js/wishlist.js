$(document).ready(function () {
    // Wishlist button click handler
    $(document).on('click', '.wishlist-btn', function (e) {
        e.preventDefault();
        
        const button = $(this);
        const productId = button.data('product-id');
        const isActive = button.hasClass('active');
        
        if (isActive) {
            removeFromWishlist(productId, button);
        } else {
            addToWishlist(productId, button);
        }
    });

    // Remove from wishlist button click handler
    $(document).on('click', '.remove-wishlist-btn', function (e) {
        e.preventDefault();
        
        const button = $(this);
        const productId = button.data('product-id');
        const card = button.closest('.wishlist-item-card');
        
        removeFromWishlist(productId, null, card);
    });

    // Move to cart button click handler
    $(document).on('click', '.move-to-cart-btn', function (e) {
        e.preventDefault();
        
        const button = $(this);
        const productId = button.data('product-id');
        const card = button.closest('.wishlist-item-card');
        
        moveToCart(productId, card);
    });

    function addToWishlist(productId, button) {
        $.ajax({
            url: '/Customer/Wishlist/AddToWishlist',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    // Update button state
                    if (button) {
                        button.addClass('active');
                        button.find('i').removeClass('far').addClass('fas');
                        button.attr('title', 'Remove from wishlist');
                    }
                    
                    // Update wishlist count in header
                    if (typeof updateWishlistBadge === 'function') {
                        updateWishlistBadge(response.wishlistCount);
                    } else {
                        updateWishlistCount(response.wishlistCount);
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

    function removeFromWishlist(productId, button, card) {
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
                                button.removeClass('active');
                                button.find('i').removeClass('fas').addClass('far');
                                button.attr('title', 'Add to wishlist');
                            }
                            // Remove card from wishlist page if present
                            if (card) {
                                card.fadeOut(300, function () {
                                    $(this).remove();
                                    if ($('.wishlist-item-card').length === 0) {
                                        location.reload();
                                    }
                                });
                            } else if (button) {
                                // Try to find and remove the card from the button context
                                var cardFromButton = button.closest('.wishlist-item-card');
                                if (cardFromButton.length) {
                                    cardFromButton.fadeOut(300, function () {
                                        $(this).remove();
                                        if ($('.wishlist-item-card').length === 0) {
                                            location.reload();
                                        }
                                    });
                                }
                            }
                            // Update wishlist count in header
                            if (typeof updateWishlistBadge === 'function') {
                                updateWishlistBadge(response.wishlistCount);
                            } else {
                                updateWishlistCount(response.wishlistCount);
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

    function moveToCart(productId, card) {
        $.ajax({
            url: '/Customer/Wishlist/MoveToCart',
            type: 'POST',
            data: { productId: productId },
            success: function (response) {
                if (response.success) {
                    // Remove card from wishlist page
                    if (card) {
                        card.fadeOut(300, function () {
                            $(this).remove();
                            // Check if wishlist is empty
                            if ($('.wishlist-item-card').length === 0) {
                                location.reload(); // Reload to show empty state
                            }
                        });
                    }
                    
                    // Update wishlist count in header
                    if (typeof updateWishlistBadge === 'function') {
                        updateWishlistBadge(response.wishlistCount);
                    } else {
                        updateWishlistCount(response.wishlistCount);
                    }
                    
                    // Update cart count in header (if cart count element exists)
                    if (typeof updateCartBadge === 'function') {
                        // Get updated cart count from response or make an AJAX call
                        $.ajax({
                            url: '/Customer/Cart/GetCartCount',
                            type: 'GET',
                            success: function (cartResponse) {
                                if (cartResponse.success) {
                                    updateCartBadge(cartResponse.count);
                                }
                            }
                        });
                    } else {
                        updateCartCount();
                    }
                    
                    // Show success message
                    Swal.fire({
                        icon: 'success',
                        title: 'Moved to Cart!',
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
                    text: 'Failed to move product to cart. Please try again.',
                });
            }
        });
    }

    function updateWishlistCount(count) {
        // Update wishlist count in header
        const wishlistBadge = $('.wishlist-badge, .modern-badge');
        if (wishlistBadge.length > 0) {
            if (count > 0) {
                wishlistBadge.text(count).show();
            } else {
                wishlistBadge.hide();
            }
        }
    }

    function updateCartCount() {
        // Update cart count in header (if cart count element exists)
        const cartBadge = $('.cart-badge, .modern-badge');
        if (cartBadge.length > 0) {
            // For now, we'll just increment the existing count
            const currentCount = parseInt(cartBadge.text()) || 0;
            cartBadge.text(currentCount + 1);
        }
    }

    // Initialize wishlist buttons on page load
    function initializeWishlistButtons() {
        $('.wishlist-btn').each(function () {
            const button = $(this);
            const productId = button.data('product-id');
            
            // Check wishlist status for each button
            $.ajax({
                url: '/Customer/Wishlist/GetWishlistStatus',
                type: 'GET',
                data: { productId: productId },
                success: function (response) {
                    if (response.inWishlist) {
                        button.addClass('active');
                        button.find('i').removeClass('far').addClass('fas');
                        button.attr('title', 'Remove from wishlist');
                    }
                }
            });
        });
    }

    // Initialize on page load
    initializeWishlistButtons();
});

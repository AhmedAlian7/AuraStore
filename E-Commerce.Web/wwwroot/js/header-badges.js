// Header Badge Management
$(document).ready(function () {
    // Don't initialize badge counts on page load
    // Badges will be updated only when cart/wishlist operations occur
});

// Function to update cart badge with animation
function updateCartBadge(newCount) {
    const cartBadge = $('.modern-icon-btn[onclick*="Cart"] .modern-badge, .modern-icon-btn[href*="Cart"] .modern-badge');
    
    if (cartBadge.length > 0) {
        const currentCount = parseInt(cartBadge.text()) || 0;
        
        if (newCount > 0) {
            if (currentCount === 0) {
                // Show badge for the first time
                cartBadge.text(newCount).show().addClass('badge-appear');
                setTimeout(() => cartBadge.removeClass('badge-appear'), 500);
            } else if (newCount !== currentCount) {
                // Animate count change
                animateBadgeCount(cartBadge, currentCount, newCount);
            }
        } else {
            // Hide badge if count is 0
            cartBadge.addClass('badge-disappear');
            setTimeout(() => {
                cartBadge.hide().removeClass('badge-disappear');
            }, 300);
        }
    }
}

// Function to update wishlist badge with animation
function updateWishlistBadge(newCount) {
    const wishlistBadge = $('.wishlist-badge, .modern-icon-btn[href*="Wishlist"] .modern-badge');
    
    if (wishlistBadge.length > 0) {
        const currentCount = parseInt(wishlistBadge.text()) || 0;
        
        if (newCount > 0) {
            if (currentCount === 0) {
                // Show badge for the first time
                wishlistBadge.text(newCount).show().addClass('badge-appear');
                setTimeout(() => wishlistBadge.removeClass('badge-appear'), 500);
            } else if (newCount !== currentCount) {
                // Animate count change
                animateBadgeCount(wishlistBadge, currentCount, newCount);
            }
        } else {
            // Hide badge if count is 0
            wishlistBadge.addClass('badge-disappear');
            setTimeout(() => {
                wishlistBadge.hide().removeClass('badge-disappear');
            }, 300);
        }
    }
}

// Function to animate badge count changes
function animateBadgeCount(badge, oldCount, newCount) {
    // Add bounce animation
    badge.addClass('badge-bounce');
    
    // Update the count with a slight delay for visual effect
    setTimeout(() => {
        badge.text(newCount);
    }, 150);
    
    // Remove animation class
    setTimeout(() => {
        badge.removeClass('badge-bounce');
    }, 500);
}



// Global functions to be called from other scripts
window.updateCartBadge = updateCartBadge;
window.updateWishlistBadge = updateWishlistBadge;

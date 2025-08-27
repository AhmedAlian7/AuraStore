$(document).ready(function () {

    // Quantity increase/decrease buttons
    $('.qty-increase').click(function () {
        var input = $(this).closest('.cart-item').find('.qty-input');
        var currentVal = parseInt(input.val(), 10) || 0;
        if (currentVal < 99) {
            input.val(currentVal + 1).trigger('change');
        }
    });

    $('.qty-decrease').click(function () {
        var input = $(this).closest('.cart-item').find('.qty-input');
        var currentVal = parseInt(input.val(), 10) || 0;
        if (currentVal > 1) {
            input.val(currentVal - 1).trigger('change');
        }
    });

    // Quantity input change event
    $('.qty-input').on('change', function () {
        var $input = $(this);
        var cartItemId = $input.data('cart-item-id');
        var newQuantity = parseInt($input.val(), 10) || 1;

        if (newQuantity < 1) {
            newQuantity = 1;
            $input.val(1);
        }

        updateQuantity(cartItemId, newQuantity, $input.closest('.cart-item'));
    });

    // Remove item button
    $('.remove-item').click(function () {
        var cartItemId = $(this).data('cart-item-id');
        var $cartItem = $(this).closest('.cart-item');

        const swal = Swal.mixin({
            customClass: {
                confirmButton: "btn btn-danger mx-2",
                cancelButton: "btn btn-light"
            },
            buttonsStyling: false
        });

        swal.fire({
            title: "Are you sure?",
            text: "You are going to delete this item from your cart!",
            icon: "warning",
            showCancelButton: true,
            confirmButtonText: "Yes, delete it!",
            cancelButtonText: "No, cancel!",
            reverseButtons: true
        }).then((result) => {
            if (result.isConfirmed) {
                removeItem(cartItemId, $cartItem, swal);
            }
        });
    });

    // Promo code application
    $('#apply-promo').click(function () {
        var promoCode = $('#promo-code-input').val().trim();
        if (promoCode) {
            applyPromoCode(promoCode);
        }
    });

    // Functions for AJAX operations
    function updateQuantity(cartItemId, quantity, $cartItem) {
        showLoading(true);

        $.ajax({
            url: '/Customer/Cart/UpdateQuantity',
            type: 'POST',
            data: { cartItemId, quantity },
            success: function (response) {
                if (response.success) {
                    if (response.itemSubtotal != null) {
                        $cartItem.find('.subtotal-amount').text(Number(response.itemSubtotal).toFixed(2));
                    }
                    updateOrderSummary(response.cartSummary);
                    // Update cart badge in header
                    if (typeof updateCartBadge === 'function') {
                        updateCartBadge(response.cartSummary.totalItems || 0);
                    }
                    showNotification('Quantity updated successfully!', 'success');
                } else {
                    showNotification('Failed to update quantity', 'error');
                }
            },
            error: function (xhr, status, error) {
                showNotification('An error occurred while updating quantity', 'error');
            },
            complete: function () {
                showLoading(false);
            }
        });
    }

    function removeItem(cartItemId, $cartItem, swal) {
        showLoading(true);

        $.ajax({
            url: '/Customer/Cart/DeleteItem',
            type: 'POST',
            data: { cartItemId },
            success: function (response) {
                if (response.success) {
                    $cartItem.fadeOut(300, function () {
                        $(this).remove();

                        if ($('.cart-item').length === 0) {
                            location.reload();
                        } else {
                            updateOrderSummary(response.cartSummary);
                            // Update cart badge in header
                            if (typeof updateCartBadge === 'function') {
                                updateCartBadge(response.cartSummary.totalItems || 0);
                            }
                        }
                    });

                    swal.fire({
                        title: "Deleted!",
                        text: "Cart item has been deleted.",
                        icon: "success"
                    });
                } else {
                    swal.fire({
                        title: "Error!",
                        text: "There was an error deleting the cart item.",
                        icon: "error"
                    });
                }
            },
            error: function (xhr, status, error) {
                showNotification('An error occurred while removing item', 'error');
                console.error('Remove item error:', error);
            },
            complete: function () {
                showLoading(false);
            }
        });
    }

    function applyPromoCode(promoCode) {
        showLoading(true);

        $.ajax({
            url: '/Customer/Cart/ApplyPromoCode',
            type: 'POST',
            data: { promoCode },
            success: function (response) {
                if (response.success) {
                    updateOrderSummary(response.cartSummary);
                    $('#promo-code-input').val('');
                    showNotification('Promo code applied successfully!', 'success');
                } else {
                    showNotification('Invalid promo code: ' + response.message, 'error');
                }
            },
            error: function (xhr, status, error) {
                showNotification('An error occurred while applying promo code', 'error');
                console.error('Apply promo code error:', error);
            },
            complete: function () {
                showLoading(false);
            }
        });
    }

    function updateOrderSummary(cartSummary) {
        if (!cartSummary) return;
        $('#total-items').text(cartSummary.totalItems || 0);
        $('#subtotal-amount').text(Number(cartSummary.subtotal || 0).toFixed(2));
        $('#tax-amount').text(Number(cartSummary.tax || 0).toFixed(2));
        $('#estimated-total').text(Number(cartSummary.total || 0).toFixed(2));
    }

    function showLoading(show) {
        $('#loading-overlay').toggleClass('d-none', !show);
    }

    const Toast = Swal.mixin({
        toast: true,
        position: 'top-end',
        showConfirmButton: false,
        timer: 3000,
        timerProgressBar: true
    });

    function showNotification(message, type) {
        Toast.fire({
            icon: type, // "success" | "error" | "warning" | "info"
            title: message
        });
    }

});

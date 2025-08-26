using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Cart;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Repositories.Implementation;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace E_Commerce.Business.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContext;
        }

        public async Task AddToCartAsync(string userId, int productId, int quantity = 1)
        {
            // Get or create Cart for user
            var cart = await _unitOfWork.Carts
                .GetByUserIdAsync(userId, "CartItems");

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await _unitOfWork.Carts.AddAsync(cart);
                await _unitOfWork.SaveAsync();
            }

            // Check if product exists
            var product = await _unitOfWork.Products.GetByIdAsync(productId);
            if (product == null)
                throw new Exception("Product not found.");

            // Check if item already in cart
            var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == productId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
                _unitOfWork.CartItems.Update(existingItem);
            }
            else
            {
                var cartItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Quantity = quantity
                };
                await _unitOfWork.CartItems.AddAsync(cartItem);
            }

            await _unitOfWork.SaveAsync();
        }

        public async Task<CartViewModel> GetUserCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts
                .GetByUserIdAsync(userId, includeProperties: "CartItems.Product")
                ?? new Cart { UserId = userId };

            var model = new CartViewModel
            {
                CartId = cart.Id,
                SubTotal = cart.TotalAmount,
                TotalItems = cart.TotalItems,
                TaxAmount = cart.TotalAmount * Numbers.TAX_RATE,
                TotalAmount = cart.TotalAmount + (cart.TotalAmount * Numbers.TAX_RATE),
                Items = cart.CartItems.Select(ci => new CartItemViewModel
                {
                    CartItemId = ci.Id,
                    ProductId = ci.ProductId,
                    ProductName = ci.Product.Name,
                    ImageUrl = ci.Product.MainImageUrl,
                    UnitPrice = ci.Product.EffectivePrice,
                    Quantity = ci.Quantity,
                }).ToList()
            };
            return model;
        }

        public async Task<UpdateQuantityResult> UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId, "Product");

            if (cartItem == null)
                throw new KeyNotFoundException("Cart item not found");

            // Prevent invalid values
            if (quantity <= 0)
                quantity = 1;

            // Check stock availability
            if (cartItem.Product != null && quantity > cartItem.Product.StockCount)
                throw new InvalidOperationException($"Only {cartItem.Product.StockCount} items available in stock.");

            cartItem.Quantity = quantity;

            _unitOfWork.CartItems.Update(cartItem);
            await _unitOfWork.SaveAsync();

            // Calculate subtotal for this specific item
            var subtotal = cartItem.Quantity * cartItem.Product.EffectivePrice;

            return new UpdateQuantityResult
            {
                Subtotal = subtotal,
                TotalAmount = 0 // Will be calculated separately via GetCartSummaryAsync
            };
        }

        public async Task DeleteItemAsync(int cartItemId)
        {
            var cartItem = await _unitOfWork.CartItems.GetByIdAsync(cartItemId);

            if (cartItem == null)
            {
                throw new ArgumentException("Cart item not found");
            }

            await _unitOfWork.CartItems.DeleteAsync(cartItemId);
            await _unitOfWork.SaveAsync();
        }

        public async Task<CartSummaryDto> GetCartSummaryAsync()
        {
            var userId = GetCurrentUserId();
            var cart = await _unitOfWork.Carts
                .GetByUserIdAsync(userId, includeProperties: "CartItems.Product,PromoCode");

            if (cart == null || !cart.CartItems.Any())
            {
                return new CartSummaryDto
                {
                    TotalItems = 0,
                    Subtotal = 0,
                    Tax = 0,
                    Total = 0
                };
            }

            // Calculate totals using EffectivePrice (discounted price if available)
            var totalItems = cart.TotalItems;
            var subtotal = cart.TotalAmount;
            var tax = subtotal * Numbers.TAX_RATE;
            
            // Apply promo code discount if exists
            var total = cart.DiscountedTotal + tax;

            return new CartSummaryDto
            {
                TotalItems = totalItems,
                Subtotal = subtotal,
                Tax = tax,
                Total = total,
                DiscountAmount = cart.DiscountAmount ?? 0
            };
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId, includeProperties: string.Empty);
            if (cart != null)
            {
                // Clear promo code when clearing cart
                cart.PromoCodeId = null;
                cart.DiscountAmount = null;
                await _unitOfWork.Carts.ClearCartAsync(userId);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<PromoCodeResult> ApplyPromoCodeAsync(string promoCode)
        {
            var result = new PromoCodeResult
            {
                Success = false,
            };

            var userId = GetCurrentUserId();
            var cart = await _unitOfWork.Carts
                .GetByUserIdAsync(userId, includeProperties: "CartItems.Product");

            if (cart == null)
            {
                result.Message = "Cart not found.";
                return result;
            }

            var code = await _unitOfWork.PromoCodes.GetByCodeAsync(promoCode);
            if (code == null || !code.IsActive)
            {
                result.Message = "Invalid or inactive promo code.";
                return result;
            }

            var now = DateTime.UtcNow;
            if (now < code.StartDate || now > code.EndDate)
            {
                result.Message = "Promo code is expired or not yet active.";
                return result;
            }

            if (code.UsageLimit.HasValue && code.UsedCount >= code.UsageLimit.Value)
            {
                result.Message = "Promo code usage limit reached.";
                return result;
            }

            decimal subtotal = cart.TotalAmount;
            if (code.MinOrderAmount.HasValue && subtotal < code.MinOrderAmount.Value)
            {
                result.Message = $"Minimum order amount is {code.MinOrderAmount.Value:C}.";
                return result;
            }

            decimal discountAmount = 0;
            if (code.DiscountType == DiscountType.Percentage)
            {
                discountAmount = subtotal * (code.DiscountValue / 100);
            }
            else if (code.DiscountType == DiscountType.FixedAmount)
            {
                discountAmount = code.DiscountValue;
            }

            // Update cart with promo code
            cart.PromoCodeId = code.Id;
            cart.PromoCode = code;
            cart.DiscountAmount = discountAmount;
            _unitOfWork.Carts.Update(cart);
            await _unitOfWork.SaveAsync();

            var cartSummary = await GetCartSummaryAsync();

            result.Success = true;
            result.Message = "Promo code applied successfully.";
            result.CartSummary = cartSummary;
            result.DiscountAmount = discountAmount;

            return result;
        }

        public async Task IncrementPromoCodeUsageAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetByUserIdAsync(userId, "");
            if (cart?.PromoCodeId != null)
            {
                var promoCode = await _unitOfWork.PromoCodes.GetByIdAsync(cart.PromoCodeId.Value);
                if (promoCode != null)
                {
                    promoCode.UsedCount++;
                    _unitOfWork.PromoCodes.Update(promoCode);
                    await _unitOfWork.SaveAsync();
                }
            }
        }
    }

}

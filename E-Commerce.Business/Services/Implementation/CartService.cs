using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Cart;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace E_Commerce.Business.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CartService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContext)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
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

            var cartItems = await _unitOfWork.CartItems
                .GetByUserIdWithProductAsync(userId);

            if (!cartItems.Any())
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
            var totalItems = cartItems.Sum(c => c.Quantity);
            var subtotal = cartItems.Sum(c => c.Quantity * c.Product.EffectivePrice);
            var tax = subtotal * Numbers.TAX_RATE;
            var total = subtotal + tax;

            return new CartSummaryDto
            {
                TotalItems = totalItems,
                Subtotal = subtotal,
                Tax = tax,
                Total = total
            };
        }
        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        }
    }

}

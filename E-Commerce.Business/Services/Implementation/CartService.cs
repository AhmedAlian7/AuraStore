using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;

namespace E_Commerce.Business.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CartService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        public async Task<Cart> GetUserCartAsync(string userId)
        {
            return await _unitOfWork.Carts
                .GetByUserIdAsync(userId, includeProperties: "CartItems.Product")
                ?? new Cart { UserId = userId };
        }
    }


}

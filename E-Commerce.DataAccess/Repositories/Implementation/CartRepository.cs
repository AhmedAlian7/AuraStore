using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(AppDbContext context) : base(context) { }

        public async Task<Cart?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartWithItemsAsync(string userId)
        {
            return await _dbSet
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }
        public async Task<Cart?> GetCartWithItemsAndProductsAsync(string userId)
        {
            return await _dbSet
                .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
                .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task ClearCartAsync(string userId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            if (cart != null)
            {
                _context.CartItems.RemoveRange(cart.CartItems);
            }
        }

        public async Task<int> GetCartItemCountAsync(string userId)
        {
            var cart = await GetCartWithItemsAsync(userId);
            return cart?.CartItems.Sum(ci => ci.Quantity) ?? 0;
        }

        public async Task<decimal> GetCartTotalAsync(string userId)
        {
            var cart = await GetCartWithItemsAndProductsAsync(userId);
            return cart?.CartItems.Sum(ci => ci.Quantity * ci.Product.EffectivePrice) ?? 0;
        }
    }
}

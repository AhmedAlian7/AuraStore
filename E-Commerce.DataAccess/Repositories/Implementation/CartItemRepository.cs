using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<CartItem?> GetByCartAndProductAsync(int cartId, int productId)
        {
            return await _dbSet.FirstOrDefaultAsync(cr => cr.CartId == cartId && cr.ProductId == productId);
        }

        public async Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId)
        {
            return await _dbSet.Where(cr => cr.CartId == cartId).ToListAsync();
        }

        public async Task UpdateQuantityAsync(int cartItemId, int quantity)
        {
            var cartItem = await GetByIdAsync(cartItemId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                _dbSet.Update(cartItem);
                //await _context.SaveChangesAsync();
            }
        }
        public async Task<IEnumerable<CartItem>> GetByUserIdWithProductAsync(string userId)
        {
            return await _dbSet
                .Include(ci => ci.Product)
                .Where(ci => ci.Cart.UserId == userId)
                .ToListAsync();
        }

    }
}

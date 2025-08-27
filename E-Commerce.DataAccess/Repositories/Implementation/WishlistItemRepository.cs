using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class WishlistItemRepository : Repository<WishlistItem>, IWishlistItemRepository
    {
        public WishlistItemRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WishlistItem>> GetUserWishlistAsync(string userId)
        {
            return await _dbSet
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();
        }

        public async Task<WishlistItem?> GetByUserAndProductAsync(string userId, int productId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task AddToWishlistAsync(string userId, int productId)
        {
            // Check if item already exists in wishlist
            var existingItem = await GetByUserAndProductAsync(userId, productId);
            if (existingItem != null)
            {
                return; // Item already in wishlist
            }

            var wishlistItem = new WishlistItem
            {
                UserId = userId,
                ProductId = productId,
                AddedAt = DateTime.UtcNow
            };

            await _dbSet.AddAsync(wishlistItem);
        }

        public async Task RemoveFromWishlistAsync(string userId, int productId)
        {
            var wishlistItem = await GetByUserAndProductAsync(userId, productId);
            if (wishlistItem != null)
            {
                _dbSet.Remove(wishlistItem);
            }
        }

        public async Task<bool> IsInWishlistAsync(string userId, int productId)
        {
            return await _dbSet
                .AnyAsync(w => w.UserId == userId && w.ProductId == productId);
        }

        public async Task<int> GetWishlistCountAsync(string userId)
        {
            return await _dbSet
                .CountAsync(w => w.UserId == userId);
        }

        public async Task<IEnumerable<WishlistItem>> GetUserWishlistWithProductsAsync(string userId)
        {
            return await _dbSet
                .Include(w => w.Product)
                    .ThenInclude(p => p.Category)
                .Include(w => w.Product)
                    .ThenInclude(p => p.ProductImages)
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.AddedAt)
                .ToListAsync();
        }
    }
}

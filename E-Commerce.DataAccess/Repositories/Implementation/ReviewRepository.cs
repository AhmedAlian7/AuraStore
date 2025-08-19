using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class ReviewRepository : Repository<Review> , IReviewRepository
    {
        public ReviewRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetByProductIdAsync(int productId)
        {
            return await _dbSet.Where(r => r.ProductId == productId).ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetByUserIdAsync(string userId)
        {
            return await _dbSet.Where(r => r.UserId == userId).ToListAsync();
        }

        public Task<bool> HasUserPurchasedProductAsync(string userId, int productId)
        {
            var query = _context.Orders
                .Where(o => o.UserId == userId && o.OrderItems.Any(oi => oi.ProductId == productId))
                .Select(o => o.Id);

            return query.AnyAsync();
        }

        public async Task<IEnumerable<Review>> GetLatestReviewsAsync(int count)
        {
            return await _context.Reviews
                    .Include(r => r.User)
                    .Where(r => r.IsVerifiedPurchase && r.Rating >= 4)
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(count)
                    .ToListAsync();
        }
    }
}

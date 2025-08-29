using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class ProductNotificationRepository : Repository<ProductNotification>, IProductNotificationRepository
    {
        public ProductNotificationRepository(AppDbContext context) : base(context) { }

        public async Task<IEnumerable<ProductNotification>> GetByProductIdAsync(int productId)
        {
            return await _context.ProductNotifications
                .Where(pn => pn.ProductId == productId && !pn.IsDeleted)
                .Include(pn => pn.User)
                .Include(pn => pn.Product)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductNotification>> GetByUserIdAsync(string userId)
        {
            return await _context.ProductNotifications
                .Where(pn => pn.UserId == userId && !pn.IsDeleted)
                .Include(pn => pn.Product)
                .Include(pn => pn.Product.Category)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProductNotification>> GetPendingNotificationsAsync()
        {
            return await _context.ProductNotifications
                .Where(pn => !pn.IsNotified && !pn.IsDeleted)
                .Include(pn => pn.User)
                .Include(pn => pn.Product)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int productId, string userId)
        {
            return await _context.ProductNotifications
                .AnyAsync(pn => pn.ProductId == productId && pn.UserId == userId && !pn.IsDeleted);
        }

        public async Task<ProductNotification?> GetByProductAndUserAsync(int productId, string userId)
        {
            return await _context.ProductNotifications
                .FirstOrDefaultAsync(pn => pn.ProductId == productId && pn.UserId == userId && !pn.IsDeleted);
        }
    }
}

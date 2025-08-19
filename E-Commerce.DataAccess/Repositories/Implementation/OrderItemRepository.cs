using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class OrderItemRepository : Repository<OrderItem>, IOrderItemRepository
    {
        public OrderItemRepository(AppDbContext context) : base(context)
        {
        }

        public Task<OrderItem?> GetByOrderAndProductAsync(int orderId, int productId)
        {
            return _dbSet
                .Include(oi => oi.Product)
                .FirstOrDefaultAsync(oi => oi.OrderId == orderId && oi.ProductId == productId);
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId)
        {
            return await _dbSet
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<bool> HasUserPurchasedProductAsync(string userId, int productId)
        {
            return await _dbSet
                .Include(oi => oi.Order)
                .AnyAsync(oi => oi.ProductId == productId && oi.Order.UserId == userId &&
                    (oi.Order.OrderStatus == OrderStatus.Delivered || oi.Order.OrderStatus == OrderStatus.Paid));
        }
    }
}

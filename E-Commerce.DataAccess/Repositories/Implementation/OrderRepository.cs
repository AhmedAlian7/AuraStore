using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Order>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetAllWithItemsAsync()
        {
            
            return await _dbSet
                 .Include(o => o.OrderItems)
                     .ThenInclude(oi => oi.Product)
                     .Where(o => !o.IsDeleted)
                 .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
        {
            return await _dbSet
                .Where(o => o.OrderStatus == status)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(string userId)
        {
            return await _dbSet
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<int> GetOrderCountByStatusAsync(OrderStatus status)
        {
            return await _dbSet.CountAsync(o => o.OrderStatus == status);
        }

        public async Task<int> GetOrderCountByUserIdAsync(string id)
        {
            return await _dbSet.CountAsync(o => o.UserId == id);
        }


        public async Task<decimal> GetTotalSalesByUserAsync(string userId)
        {
            return await _dbSet
                .Where(o => o.UserId == userId && o.OrderStatus == OrderStatus.Delivered)
                .SumAsync(o => o.TotalAmount);
        }

        public async Task UpdateOrderStatusAsync(int orderId, OrderStatus status)
        {
            var order = await GetByIdAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = status;
                _dbSet.Update(order);
                //await _context.SaveChangesAsync();
            }
        }
    }
}


using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<IEnumerable<Order>> GetByUserIdAsync(string userId);
        Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
        Task<decimal> GetTotalSalesByUserAsync(string userId);
        Task<int> GetOrderCountByStatusAsync(OrderStatus status);
        Task UpdateOrderStatusAsync(int orderId, OrderStatus status);
    }
}

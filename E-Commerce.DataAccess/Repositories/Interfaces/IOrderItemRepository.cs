
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task<OrderItem?> GetByOrderAndProductAsync(int orderId, int productId);
    }
}

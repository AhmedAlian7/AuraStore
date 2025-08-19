using E_Commerce.DataAccess.Entities;
using System.Threading.Tasks;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IOrderItemRepository : IRepository<OrderItem>
    {
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(int orderId);
        Task<OrderItem?> GetByOrderAndProductAsync(int orderId, int productId);
        Task<bool> HasUserPurchasedProductAsync(string userId, int productId);
    }
}

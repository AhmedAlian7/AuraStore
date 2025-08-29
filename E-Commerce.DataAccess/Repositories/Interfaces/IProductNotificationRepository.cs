using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IProductNotificationRepository : IRepository<ProductNotification>
    {
        Task<IEnumerable<ProductNotification>> GetByProductIdAsync(int productId);
        Task<IEnumerable<ProductNotification>> GetByUserIdAsync(string userId);
        Task<IEnumerable<ProductNotification>> GetPendingNotificationsAsync();
        Task<bool> ExistsAsync(int productId, string userId);
        Task<ProductNotification?> GetByProductAndUserAsync(int productId, string userId);
    }
}

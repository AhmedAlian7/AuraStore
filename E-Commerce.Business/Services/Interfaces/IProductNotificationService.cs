using E_Commerce.DataAccess.Entities;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IProductNotificationService
    {
        Task<bool> AddNotificationAsync(int productId, string userId, string email);
        Task<List<ProductNotification>> GetPendingNotificationsAsync(int productId);
        Task MarkAsNotifiedAsync(List<int> notificationIds);
        Task<bool> RemoveNotificationAsync(int productId, string userId);
        Task<List<ProductNotification>> GetUserNotificationsAsync(string userId);
    }
}

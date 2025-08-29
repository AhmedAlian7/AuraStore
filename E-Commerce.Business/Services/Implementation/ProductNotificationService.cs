using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;

namespace E_Commerce.Business.Services.Implementation
{
    public class ProductNotificationService : IProductNotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductNotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddNotificationAsync(int productId, string userId, string email)
        {
            try
            {
                // Check if notification already exists
                var existingNotification = await _unitOfWork.ProductNotifications.GetByProductAndUserAsync(productId, userId);
                if (existingNotification != null)
                {
                    return false; // Notification already exists
                }

                // Create new notification
                var notification = new ProductNotification
                {
                    ProductId = productId,
                    UserId = userId,
                    IsNotified = false,
                    RequestDate = DateTime.UtcNow
                };

                await _unitOfWork.ProductNotifications.AddAsync(notification);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ProductNotification>> GetPendingNotificationsAsync(int productId)
        {
            var notifications = await _unitOfWork.ProductNotifications.GetByProductIdAsync(productId);
            return notifications.Where(n => !n.IsNotified).ToList();
        }

        public async Task MarkAsNotifiedAsync(List<int> notificationIds)
        {
            foreach (var id in notificationIds)
            {
                var notification = await _unitOfWork.ProductNotifications.GetByIdAsync(id);
                if (notification != null)
                {
                    notification.IsNotified = true;
                    _unitOfWork.ProductNotifications.Update(notification);
                }
            }
            await _unitOfWork.SaveAsync();
        }

        public async Task<bool> RemoveNotificationAsync(int productId, string userId)
        {
            try
            {
                var notification = await _unitOfWork.ProductNotifications.GetByProductAndUserAsync(productId, userId);
                if (notification != null)
                {
                    await _unitOfWork.ProductNotifications.DeleteAsync(notification.Id);
                    await _unitOfWork.SaveAsync();
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<ProductNotification>> GetUserNotificationsAsync(string userId)
        {
            var notifications = await _unitOfWork.ProductNotifications.GetByUserIdAsync(userId);
            return notifications.ToList();
        }
    }
}

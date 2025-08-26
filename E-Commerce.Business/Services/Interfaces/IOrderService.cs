using E_Commerce.Business.ViewModels.Payment;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
namespace E_Commerce.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order?> GetOrderAsync(string orderId, string? userId = null);
        Task UpdateOrderStatusAsync(string orderId, OrderStatus status);
        Task UpdatePaymentDetailsAsync(string orderId, string paymentIntentId, string sessionId, decimal? amount, string currency);
        Task<Order> CreateOrderFromCartAsync(string userId);


    }
}

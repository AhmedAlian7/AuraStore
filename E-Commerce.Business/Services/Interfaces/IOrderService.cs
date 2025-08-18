using E_Commerce.Business.ViewModels.Payment;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
namespace E_Commerce.Business.Services.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateDraftOrderAsync(string userId, string shippingAddressId, string? discountCode = null);
        Task<Order?> GetOrderAsync(string orderId, string? userId = null);
        Task UpdateOrderStatusAsync(string orderId, OrderStatus status);
        Task UpdatePaymentDetailsAsync(string orderId, string paymentIntentId, string sessionId, decimal? amount, string currency);
        Task<CheckoutViewModel> GetCheckoutDataAsync(string userId, string? discountCode = null);
        Task<Order> CreateOrderFromCartAsync(string userId);
    }
}

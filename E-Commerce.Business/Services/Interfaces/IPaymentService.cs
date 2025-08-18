using E_Commerce.Business.ViewModels.Payment;
namespace E_Commerce.Business.Services.Interfaces
{
    public interface IPaymentService
    {
        Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(CreateCheckoutSessionRequest request, string userId);
        Task<PaymentStatusViewModel> GetPaymentStatusAsync(string sessionId);
        Task<PaymentStatusViewModel> GetOrderStatusAsync(string orderId, string userId);
    }
}

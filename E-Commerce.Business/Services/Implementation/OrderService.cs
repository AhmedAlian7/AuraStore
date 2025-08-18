using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Payment;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services.Implementation
{
    public class OrderService : IOrderService
    {

       public async Task<Order> CreateDraftOrderAsync(string userId, string shippingAddressId, string? discountCode = null) { return null; }
       public async Task<Order?> GetOrderAsync(string orderId, string? userId = null) { return null; }
       public async Task UpdateOrderStatusAsync(string orderId, OrderStatus status) {  }
       public async Task UpdatePaymentDetailsAsync(string orderId, string paymentIntentId, string sessionId, decimal? amount, string currency) {  }
       public async Task<CheckoutViewModel> GetCheckoutDataAsync(string userId, string? discountCode = null) { return null; }
    }



}

using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _cartRepository = unitOfWork.Carts;
            _orderRepository = unitOfWork.Orders;
            _orderItemRepository = unitOfWork.OrderItems;
        }

        public async Task<Order> CreateOrderFromCartAsync(string userId)
        {
            var cart = await _cartRepository.GetCartWithItemsAndProductsAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            var order = new Order
            {
                UserId = userId,
                OrderStatus = OrderStatus.PendingPayment,
                SubTotal = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.EffectivePrice),
                TaxAmount = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.EffectivePrice) * 0.1m, // Example tax
                DiscountAmount = 0,
                ShippingCost = 0,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.EffectivePrice,
                    Product = ci.Product // Attach product reference for later use
                }).ToList()
            };

            await _orderRepository.AddAsync(order);
            await _unitOfWork.SaveAsync();

            // Clear cart items
            await _cartRepository.ClearCartAsync(userId);
            await _unitOfWork.SaveAsync();

            return order;
        }

        public async Task UpdateOrderStatusAsync(string orderId, OrderStatus status)
        {
            if (!int.TryParse(orderId, out var id))
                throw new ArgumentException("Invalid order id");
            var order = await _orderRepository.GetByIdAsync(id, "OrderItems,OrderItems.Product");
            if (order == null)
                throw new InvalidOperationException("Order not found");
            order.OrderStatus = status;
            await _unitOfWork.SaveAsync();
        }

        // Other IOrderService methods (not implemented here)
        public Task<Order> CreateDraftOrderAsync(string userId, string shippingAddressId, string? discountCode = null) => throw new NotImplementedException();
        public async Task<Order?> GetOrderAsync(string orderId, string? userId = null)
        {
            if (!int.TryParse(orderId, out var id))
                throw new ArgumentException("Invalid order id");
            var order = await _orderRepository.GetByIdAsync(id, "OrderItems,OrderItems.Product");
            if (order == null)
                return null;
            if (userId != null && order.UserId != userId)
                return null;
            return order;
        }

        public Task UpdatePaymentDetailsAsync(string orderId, string paymentIntentId, string sessionId, decimal? amount, string currency)
        {
            // Dummy implementation for compatibility
            return Task.CompletedTask;
        }

        public Task<E_Commerce.Business.ViewModels.Payment.CheckoutViewModel> GetCheckoutDataAsync(string userId, string? discountCode = null) => throw new NotImplementedException();
    }
}

using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Repositories.Interfaces;

namespace E_Commerce.Business.Services.Implementation
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Order> CreateOrderFromCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItemsAndProductsAsync(userId);
            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
                throw new InvalidOperationException("Cart is empty.");

            var order = new Order
            {
                UserId = userId,
                OrderStatus = OrderStatus.PendingPayment,
                SubTotal = cart.CartItems.Sum(ci => ci.Quantity * ci.Product.EffectivePrice),
                TaxAmount = 0.00m,
                DiscountAmount = cart.DiscountAmount ?? 0,
                ShippingCost = 0,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.Product.EffectivePrice,
                    Product = ci.Product // Attach product reference for later use
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveAsync();

            return order;
        }

        public async Task UpdateOrderStatusAsync(string orderId, OrderStatus status)
        {
            if (!int.TryParse(orderId, out var id))
                throw new ArgumentException("Invalid order id");
            var order = await _unitOfWork.Orders.GetByIdAsync(id, "OrderItems,OrderItems.Product");
            if (order == null)
                throw new InvalidOperationException("Order not found");
            order.OrderStatus = status;
            await _unitOfWork.SaveAsync();


            foreach (var orderItem in order.OrderItems)
            {
                var product = orderItem.Product;
                if (product != null)
                {
                    product.StockCount -= orderItem.Quantity;
                    if (product.StockCount < 0)
                        product.StockCount = 0;
                    _unitOfWork.Products.Update(product);
                }
            }
            await _unitOfWork.SaveAsync();
            
        }
        public async Task<Order?> GetOrderAsync(string orderId, string? userId = null)
        {
            if (!int.TryParse(orderId, out var id))
                throw new ArgumentException("Invalid order id");
            var order = await _unitOfWork.Orders.GetByIdAsync(id, "OrderItems,OrderItems.Product,User");
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

    }
}

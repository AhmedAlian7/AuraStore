using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Payment;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using E_Commerce.Business.Configuration;

namespace E_Commerce.Business.Services.Implementation
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _orderService;
        private readonly ILogger<PaymentService> _logger;
        private readonly StripeSettings _stripeSettings;

        public PaymentService(
            IUnitOfWork unitOfWork,
            IOrderService orderService,
            ILogger<PaymentService> logger,
            IOptions<StripeSettings> stripeSettings)
        {
            _unitOfWork = unitOfWork;
            _orderService = orderService;
            _logger = logger;
            _stripeSettings = stripeSettings.Value;
            StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
        }

        public async Task<CheckoutSessionResponse> CreateCheckoutSessionAsync(
            CreateCheckoutSessionRequest request,
            string userId)
        {
            try
            {   // include "OrderItems.Product,User"
                var order = await _orderService.GetOrderAsync(request.OrderId, userId); 
                if (order == null)
                {
                    return new CheckoutSessionResponse
                    {
                        Success = false,
                        ErrorMessage = "Order not found"
                    };
                }

                if (order.OrderStatus != OrderStatus.Draft && order.OrderStatus != OrderStatus.PendingPayment)
                {
                    return new CheckoutSessionResponse
                    {
                        Success = false,
                        ErrorMessage = "Order is not in a valid state for payment"
                    };
                }

                var lineItems = order.OrderItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.UnitPrice * 100), // Convert to cents
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Description = item.Product.Description,
                            Images = !string.IsNullOrEmpty(item.Product.MainImageUrl)
                                ? new List<string> { item.Product.MainImageUrl}
                                : null
                        }
                    },
                    Quantity = item.Quantity
                }).ToList();

                // Add shipping as a line item if applicable
                //if (order.ShippingAmount > 0)
                //{
                //    lineItems.Add(new SessionLineItemOptions
                //    {
                //        PriceData = new SessionLineItemPriceDataOptions
                //        {
                //            UnitAmount = (long)(order.ShippingAmount * 100),
                //            Currency = "usd",
                //            ProductData = new SessionLineItemPriceDataProductDataOptions
                //            {
                //                Name = "Shipping"
                //            }
                //        },
                //        Quantity = 1
                //    });
                //}

                var sessionOptions = new SessionCreateOptions
                {
                    PaymentMethodTypes = new List<string> { "card" },
                    LineItems = lineItems,
                    Mode = "payment",
                    SuccessUrl = request.SuccessUrl + "?session_id={CHECKOUT_SESSION_ID}",
                    CancelUrl = request.CancelUrl,
                    Metadata = new Dictionary<string, string>
                    {
                        { "orderId", order.Id.ToString() },
                        { "userId", userId }
                    },
                    CustomerEmail = order.User.Email,
                    BillingAddressCollection = "auto",
                    ShippingAddressCollection = new SessionShippingAddressCollectionOptions
                    {
                        AllowedCountries = new List<string> { "US", "CA", "GB", "AU", "EG" }
                    }
                };

                // Add discounts if applicable
                //if (order.DiscountAmount > 0)
                //{
                //    sessionOptions.Discounts = new List<SessionDiscountOptions>
                //    {
                //        new SessionDiscountOptions
                //        {
                //            Coupon = order.DiscountCode
                //        }
                //    };
                //}

                var service = new SessionService();
                var session = await service.CreateAsync(sessionOptions);

                // Update order with session information
                //order.StripeSessionId = session.Id;
                order.OrderStatus = OrderStatus.PendingPayment;
                order.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.SaveAsync();

                return new CheckoutSessionResponse
                {
                    Success = true,
                    SessionId = session.Id,
                    CheckoutUrl = session.Url,
                    OrderId = order.Id.ToString()
                };
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe error creating checkout session for order {OrderId}", request.OrderId);
                return new CheckoutSessionResponse
                {
                    Success = false,
                    ErrorMessage = $"Payment processing error: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating checkout session for order {OrderId}", request.OrderId);
                return new CheckoutSessionResponse
                {
                    Success = false,
                    ErrorMessage = "An error occurred while processing your request"
                };
            }
        }

        public async Task<PaymentStatusViewModel> GetPaymentStatusAsync(string sessionId)
        {
            try
            {
                var service = new SessionService();
                var session = await service.GetAsync(sessionId);

                var orderId = session.Metadata.GetValueOrDefault("orderId");
                if (string.IsNullOrEmpty(orderId))
                {
                    throw new InvalidOperationException("Order ID not found in session metadata");
                }

                var order = await _orderService.GetOrderAsync(orderId); // include "OrderItems.Product,User"
                if (order == null)
                {
                    throw new InvalidOperationException("Order not found");
                }

                return new PaymentStatusViewModel
                {
                    OrderId = order.Id.ToString(),
                    Status = MapOrderStatus(order.OrderStatus),
                    Amount = order.TotalAmount,
                    Currency = "usd",
                    //PaymentIntentId = order.StripePaymentIntentId,
                    SessionId = session.Id,
                    //PaidAt = order.PaidAt,
                    CustomerEmail = order.User.Email,
                    Items = order.OrderItems.Select(i => new OrderItemViewModel
                    {
                        ProductName = i.Product.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Total = i.Quantity * i.UnitPrice
                    }).ToList()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment status for session {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<PaymentStatusViewModel> GetOrderStatusAsync(string orderId, string userId)
        {
            var order = await _orderService.GetOrderAsync(orderId, userId);
            if (order == null)
            {
                throw new InvalidOperationException("Order not found");
            }

            return new PaymentStatusViewModel
            {
                OrderId = order.Id.ToString()   ,
                Status = MapOrderStatus(order.OrderStatus),
                Amount = order.TotalAmount,
                Currency = "usd",
                //PaymentIntentId = order.StripePaymentIntentId,
                //SessionId = order.StripeSessionId,
                //PaidAt = order.PaidAt,
                CustomerEmail = order.User.Email,
                Items = order.OrderItems.Select(i => new OrderItemViewModel
                {
                    ProductName = i.Product.Name,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Total = i.UnitPrice * i.Quantity
                }).ToList()
            };
        }
        private async Task HandleCheckoutSessionCompleted(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return;

            var orderId = session.Metadata.GetValueOrDefault("orderId");
            if (string.IsNullOrEmpty(orderId))
            {
                _logger.LogWarning("Order ID not found in completed session metadata: {SessionId}", session.Id);
                return;
            }

            var order = await _orderService.GetOrderAsync(orderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found for completed session: {OrderId}", orderId);
                return;
            }

            // Update order to paid status
            await _orderService.UpdatePaymentDetailsAsync(
                orderId,
                session.PaymentIntentId,
                session.Id,
                session.AmountTotal / 100m, // Convert from cents
                session.Currency.ToUpper()
            );

            await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Paid);

            _logger.LogInformation("Order {OrderId} marked as paid from session {SessionId}", orderId, session.Id);

            // TODO: Trigger fulfillment process (email, inventory update, etc.)
        }

        private void HandlePaymentIntentSucceeded(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            _logger.LogInformation("Payment intent succeeded: {PaymentIntentId}", paymentIntent.Id);
            // Additional processing if needed
        }

        private void HandlePaymentIntentFailed(Event stripeEvent)
        {
            var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
            if (paymentIntent == null) return;

            _logger.LogWarning("Payment intent failed: {PaymentIntentId}", paymentIntent.Id);

            // Try to find the order and mark as failed
            // This might require additional metadata or database queries
        }

        private async Task HandleCheckoutSessionExpired(Event stripeEvent)
        {
            var session = stripeEvent.Data.Object as Session;
            if (session == null) return;

            var orderId = session.Metadata.GetValueOrDefault("orderId");
            if (!string.IsNullOrEmpty(orderId))
            {
                await _orderService.UpdateOrderStatusAsync(orderId, OrderStatus.Cancelled);
                _logger.LogInformation("Order {OrderId} cancelled due to expired session {SessionId}",
                    orderId, session.Id);
            }
        }

        private string MapOrderStatus(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.Draft => "draft",
                OrderStatus.PendingPayment => "pending_payment",
                OrderStatus.Paid => "paid",
                OrderStatus.Processing => "processing",
                OrderStatus.Shipped => "shipped",
                OrderStatus.Delivered => "delivered",
                OrderStatus.Cancelled => "cancelled",
                OrderStatus.Returned => "refunded",
                _ => "unknown"
            };
        }
    }

}

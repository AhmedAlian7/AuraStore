using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Cart;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Threading.Tasks;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly string _stripeSecretKey;
        private readonly string _domain;
        private readonly UserManager<ApplicationUser> _userManager;

        public CheckoutController(ICartService cartService, IOrderService orderService, IEmailService emailService, IConfiguration configuration, UserManager<ApplicationUser> applicationUser)
        {
            _cartService = cartService;
            _orderService = orderService;
            _emailService = emailService;
            _stripeSecretKey = configuration["Stripe:SecretKey"];
            _domain = configuration["Stripe:Domain"] ?? "https://localhost:7167";
            _userManager = applicationUser;

        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult PaymentMethod(string option)
        {
            if (option == "CreditCard")
                return RedirectToAction("CreateCheckoutSession");
            else
                return RedirectToAction("Recipt");
        }

        public async Task<IActionResult> Recipt()
        {
            var model = await _cartService.GetCartSummaryAsync();
            // Do not overwrite tax or subtotal, just pass delivery tax as ViewBag
            decimal deliveryTax = 12.00m; // Only for pay on delivery
            ViewBag.DeliveryTax = deliveryTax;
            ViewBag.GrandTotal = model.Subtotal + model.Tax + deliveryTax;
            return View(model);
        }

        // POST: Process the receipt form submission
        [HttpPost]
        public async Task<IActionResult> Recipt(CartSummaryDto model, decimal DeliveryTax)
        {
            if (!ModelState.IsValid)
            {
                var cartData = await _cartService.GetCartSummaryAsync();
                model.TotalItems = cartData.TotalItems;
                model.Subtotal = cartData.Subtotal;
                model.Tax = cartData.Tax;
                ViewBag.DeliveryTax = DeliveryTax;
                ViewBag.GrandTotal = model.Subtotal + model.Tax + DeliveryTax;
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Create order from cart (with payment upon receipt)
            var order = await _orderService.CreateOrderFromCartAsync(user.Id);
            // Set order status to Pending (for cash on delivery)
            await _orderService.UpdateOrderStatusAsync(order.Id.ToString(), E_Commerce.DataAccess.Enums.OrderStatus.Pending);

            // Optionally send confirmation email here
            if (order.User != null)
            {
                var emailBody = $@"<h2>Thank you for your order!</h2><p>Order ID: {order.Id}</p><ul>" +
                    string.Join("", order.OrderItems.Select(item => $"<li>{item.Product.Name} x {item.Quantity} - ${item.UnitPrice * item.Quantity:F2}</li>")) +
                    $"</ul><p><strong>Total: ${(order.TotalAmount + DeliveryTax):F2} (includes delivery tax)</strong></p>";
                await _emailService.SendEmailAsync(order.User.Email, "Your Order Confirmation", emailBody);
            }

            // Redirect to success page
            return RedirectToAction("Success", new { orderId = order.Id });
        }


        // This action will be triggered when the user clicks "Proceed to Pay"
        [HttpGet]
        public async Task<IActionResult> CreateCheckoutSession()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            // Create order from cart
            var order = await _orderService.CreateOrderFromCartAsync(user.Id);

            // Load products for order items
            foreach (var item in order.OrderItems)
            {
                _ = item.Product ?? throw new InvalidOperationException("OrderItem.Product must be loaded.");
            }

            var options = new SessionCreateOptions
            {
                LineItems = order.OrderItems.Select(item => new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.UnitPrice * 100),
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                            Images = new List<string> { item.Product.MainImageUrl }
                        }
                    },
                    Quantity = item.Quantity,
                }).ToList(),
                Mode = "payment",
                SuccessUrl = $"{_domain}/Customer/Checkout/Success?orderId={order.Id}&session_id={{CHECKOUT_SESSION_ID}}",
                CancelUrl = $"{_domain}/Customer/Checkout/Cancel",
            };

            var client = new Stripe.StripeClient(_stripeSecretKey);
            var service = new SessionService(client);
            var session = service.Create(options);

            return Redirect(session.Url);
        }

        public async Task<IActionResult> Success(int orderId, string session_id)
        {
            bool isPaid = !string.IsNullOrEmpty(session_id);
            if (isPaid)
            {
                await _orderService.UpdateOrderStatusAsync(orderId.ToString(), OrderStatus.Paid);
            }
            var order = await _orderService.GetOrderAsync(orderId.ToString());
            if (order != null && order.User != null)
            {
                var paymentStatusText = isPaid
                    ? "<strong>Status:</strong> Paid online via Credit Card."
                    : "<strong>Status:</strong> Payment due upon delivery.";

                var emailBody = $@"
                    <div style='font-family: Arial, sans-serif; color: #333;'>
                        <h2 style='color:#4CAF50;'>Order Confirmation</h2>
                        <p>Dear {order.User.Email.Split('@')[0]},</p>
                        <p>Thank you for shopping with us! We are pleased to confirm that we have received your order.</p>
        
                        <p><strong>Order ID:</strong> {order.Id}</p>
        
                        <h3>Order Details:</h3>
                        <ul style='list-style-type:none; padding:0;'>
                            {string.Join("", order.OrderItems.Select(item =>
                                                $"<li>{item.Product.Name} &times; {item.Quantity} - ${item.UnitPrice * item.Quantity:F2}</li>"))}
                        </ul>
        
                        <p><strong>Total Amount:</strong> ${order.TotalAmount:F2}</p>
                        <p>{paymentStatusText}</p>

                        <p>We will notify you once your order has been shipped.</p>
                        <p style='margin-top:20px;'>Best regards,<br/>The Aura Store Team</p>
                    </div>";
                await _emailService.SendEmailAsync(order.User.Email, "Your Order Confirmation", emailBody);
            }
            ViewData["SuccessMessage"] = isPaid ? "Payment successful!" : "Order placed! Please pay upon delivery.";
            return View();
        }

        public async Task<IActionResult> Cancel()
        {
            ViewData["ErrorMessage"] = "Payment canceled.";

            return View();
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            // Redirect to Success for now (or render a confirmation view)
            return RedirectToAction("Success", new { orderId });
        }
    }
}

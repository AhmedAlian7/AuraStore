using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Cart;
using E_Commerce.DataAccess.Entities;
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
            return View(model);
        }

        // POST: Process the receipt form submission
        [HttpPost]
        public async Task<IActionResult> Recipt(CartSummaryDto model)
        {
            if (!ModelState.IsValid)
            {
                // Reload cart data if validation fails
                var cartData = await _cartService.GetCartSummaryAsync();
                model.TotalItems = cartData.TotalItems;
                model.Subtotal = cartData.Subtotal;
                model.Tax = cartData.Tax;
                model.Total = cartData.Total;
                return View(model);
            }

            // Process the order here
            // Save shipping info, create order, etc.

            return RedirectToAction("OrderConfirmation", new { id = "some-order-id" });
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
            // Update order status to Paid
            await _orderService.UpdateOrderStatusAsync(orderId.ToString(), E_Commerce.DataAccess.Enums.OrderStatus.Paid);
            var order = await _orderService.GetOrderAsync(orderId.ToString());
            if (order != null && order.User != null)    
            {
                var emailBody = $@"<h2>Thank you for your order!</h2><p>Order ID: {order.Id}</p><ul>" +
                    string.Join("", order.OrderItems.Select(item => $"<li>{item.Product.Name} x {item.Quantity} - ${item.UnitPrice * item.Quantity:F2}</li>")) +
                    $"</ul><p><strong>Total: ${order.TotalAmount:F2}</strong></p>";
                await _emailService.SendEmailAsync(order.User.Email, "Your Order Confirmation", emailBody);
            }
            ViewBag.PaymentStatus = "Payment successful!";
            return View();
        }

        public async Task<IActionResult> Cancel()
        {
            ViewBag.PaymentStatus = "Payment canceled.";
            
            return View();
        }


    }
}

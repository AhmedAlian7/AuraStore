using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Cart;
using E_Commerce.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CheckoutController : Controller
    {

        private readonly ICartService _cartService;

        public CheckoutController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PaymentMethod(string option)
        {
            if (option == "CreditCard")
                return View("CreditCard");
            else
                return View("Recipt");
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
    }
}

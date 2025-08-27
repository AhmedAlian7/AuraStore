using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;

        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var cart = await _cartService.GetUserCartAsync(user.Id);
            var cartSummary = await _cartService.GetCartSummaryAsync();
            ViewBag.DiscountAmount = cartSummary.DiscountAmount;
            return View(cart);
        }

        // for AJAX calls
        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var updateResult = await _cartService.UpdateQuantityAsync(cartItemId, quantity);
                var cartSummary = await _cartService.GetCartSummaryAsync();

                return Json(new
                {
                    success = true,
                    itemSubtotal = updateResult.Subtotal,
                    cartSummary = new
                    {
                        totalItems = cartSummary.TotalItems,
                        subtotal = cartSummary.Subtotal,
                        tax = cartSummary.Tax,
                        total = cartSummary.Total
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItem(int cartItemId)
        {
            try
            {

                await _cartService.DeleteItemAsync(cartItemId);

                // Get the updated cart summary after item deletion
                var cartSummary = await _cartService.GetCartSummaryAsync();

                return Json(new
                {
                    success = true,
                    cartSummary = new
                    {
                        totalItems = cartSummary.TotalItems,
                        subtotal = cartSummary.Subtotal,
                        tax = cartSummary.Tax,
                        total = cartSummary.Total
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> ApplyPromoCode(string promoCode)
        {
            if(string.IsNullOrWhiteSpace(promoCode))
            {
                return Json(new { success = false, message = "Promo code is required." });
            }

            var result = await _cartService.ApplyPromoCodeAsync(promoCode);

            if (result == null || !result.Success)
            {
                return Json(new
                {
                    success = false,
                    message = result?.Message ?? "Invalid or expired promo code."
                });
            }

            return Json(new
            {
                success = true,
                message = "Promo code applied successfully!",
                discountAmount = result.DiscountAmount,
                cartSummary = new
                {
                    totalItems = result.CartSummary.TotalItems,
                    subtotal = result.CartSummary.Subtotal,
                    tax = result.CartSummary.Tax,
                    total = result.CartSummary.Total
                }
            });


        }
        

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            try
            {
                var cartSummary = await _cartService.GetCartSummaryAsync();
                return Json(new { success = true, count = cartSummary.TotalItems });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }

}

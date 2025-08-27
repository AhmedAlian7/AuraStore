using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Wishlist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class WishlistController : Controller
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var wishlist = await _wishlistService.GetUserWishlistAsync(userId, page);
            return View(wishlist);
        }

        [HttpPost]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "User not authenticated" });

            var result = await _wishlistService.AddToWishlistAsync(userId, productId);
            
            if (result)
            {
                var count = await _wishlistService.GetWishlistCountAsync(userId);
                return Json(new { success = true, message = "Product added to wishlist", count = count });
            }
            
            return Json(new { success = false, message = "Product is already in wishlist" });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "User not authenticated" });

            var result = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
            
            if (result)
            {
                var count = await _wishlistService.GetWishlistCountAsync(userId);
                return Json(new { success = true, message = "Product removed from wishlist", count = count });
            }
            
            return Json(new { success = false, message = "Failed to remove product from wishlist" });
        }

        [HttpPost]
        public async Task<IActionResult> MoveToCart(int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Json(new { success = false, message = "User not authenticated" });

            var result = await _wishlistService.MoveToCartAsync(userId, productId);
            
            if (result)
            {
                var count = await _wishlistService.GetWishlistCountAsync(userId);
                return Json(new { success = true, message = "Product moved to cart", count = count });
            }
            
            return Json(new { success = false, message = "Failed to move product to cart" });
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlistStatus(int productId)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Json(new { inWishlist = false });

            var inWishlist = await _wishlistService.IsInWishlistAsync(userId, productId);
            return Json(new { inWishlist = inWishlist });
        }

        [HttpGet]
        public async Task<IActionResult> GetWishlistCount()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Json(new { count = 0 });

            var count = await _wishlistService.GetWishlistCountAsync(userId);
            return Json(new { count = count });
        }
    }
}

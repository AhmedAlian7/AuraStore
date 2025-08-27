using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using E_Commerce.Web.Helpers;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitIfWork;
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductController(IProductService productService, IUnitOfWork unitOfWork
            , ICartService cartService, IWishlistService wishlistService
            , UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _unitIfWork = unitOfWork;
            _cartService = cartService;
            _wishlistService = wishlistService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string search = null, int? category = null, string sortBy = null)
        {
            var products = await _productService.GetAllAsync(page, search, category, sortBy);
            // Fetch all categories and map to SelectListItem for dropdown
            var categories = _unitIfWork.Categories.GetAll()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = category.HasValue && x.Id == category.Value
                }).ToList();
            ViewBag.Categories = categories;

            // Add wishlist count for authenticated users
            if (User.Identity?.IsAuthenticated == true)
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (!string.IsNullOrEmpty(userId))
                {
                    var wishlistCount = await _wishlistService.GetWishlistCountAsync(userId);
                    ViewBag.WishlistCount = wishlistCount;
                }
            }

            return View("Index", products);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var productDetailsModel = await _productService.GetProductDetailsAsync(id);
            if (productDetailsModel == null)
            {
                return NotFound();
            }

            // Add wishlist status and count for authenticated users
            if (User.Identity?.IsAuthenticated == true)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var isInWishlist = await _wishlistService.IsInWishlistAsync(user.Id, id);
                    ViewBag.IsInWishlist = isInWishlist;
                    
                    var wishlistCount = await _wishlistService.GetWishlistCountAsync(user.Id);
                    ViewBag.WishlistCount = wishlistCount;
                }
            }

            return View("Details", productDetailsModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Unauthorized();

                await _cartService.AddToCartAsync(user.Id, productId, quantity);
                
                TempData["SuccessMessage"] = "Product added to cart successfully!";
                return RedirectToAction("Details", "Product", new { id = productId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Details", "Product", new { id = productId });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCartAjax(int productId, int quantity = 1)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                    return Json(new { success = false, message = "User not authenticated" });

                await _cartService.AddToCartAsync(user.Id, productId, quantity);
                
                // Get updated cart count
                var cartSummary = await _cartService.GetCartSummaryAsync();

                return Json(new { 
                    success = true, 
                    message = "Product added to cart successfully!",
                    cartCount = cartSummary.TotalItems
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Ajax call method
        [HttpPost]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            await _productService.AddReviewAsync(productId, user.Id, rating, comment);

            var productDetails = await _productService.GetProductDetailsAsync(productId);

            // Return partials for AJAX update
            return Json(new
            {
                reviewsHtml = await ControllerExtensions.RenderViewAsync(this, "_ReviewsListPartial", productDetails.Reviews, true),
                ratingHtml = await ControllerExtensions.RenderViewAsync(this, "_RatingSummaryPartial", productDetails, true)
            });
        }

    }
}

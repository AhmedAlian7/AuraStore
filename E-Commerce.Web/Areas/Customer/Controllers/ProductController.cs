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
        private readonly UserManager<ApplicationUser> _userManager;
        public ProductController(IProductService productService, IUnitOfWork unitOfWork
            , ICartService cartService
            , UserManager<ApplicationUser> userManager)
        {
            _productService = productService;
            _unitIfWork = unitOfWork;
            _cartService = cartService;
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
            return View("Details", productDetailsModel);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            await _cartService.AddToCartAsync(user.Id, productId, quantity);

            TempData["SuccessMessage"] = "Product added to cart successfully!";
            return RedirectToAction("Details", "Product", new { id = productId });
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

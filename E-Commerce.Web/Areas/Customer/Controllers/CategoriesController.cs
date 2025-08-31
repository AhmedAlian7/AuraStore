using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Category;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;

        public CategoriesController(ICategoryService categoryService, IProductService productService)
        {
            _categoryService = categoryService;
            _productService = productService;
        }

        // GET: /Categories
        public async Task<IActionResult> Index(string? search, int page = 1)
        {
            try
            {
                if (page < 1) page = 1;

                var viewModel = await _categoryService.GetAllCategoriesAsync(page, search);
                
                ViewBag.SearchTerm = search;
                ViewBag.CurrentPage = page;
                
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading categories. Please try again.";
                return View(new CategoryListViewModel());
            }
        }

        // GET: /Categories/Details/{id}
        public async Task<IActionResult> Details(int id, string? search, string? sortBy, int page = 1)
        {
            try
            {
                if (page < 1) page = 1;

                // Validate category exists
                if (!await _categoryService.CategoryExistsAsync(id))
                {
                    TempData["Error"] = "Category not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = await _productService.GetAllAsync(page, search,id, sortBy);
                
                ViewBag.SearchTerm = search;
                ViewBag.SortBy = sortBy;
                ViewBag.CurrentPage = page;
                ViewBag.CategoryId = id;
                
                return RedirectToAction("Index", "Product", new { area = "Customer", category = id, page, search, sortBy });
            }
            catch (ArgumentException ex)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while loading category details. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: /Categories/Search
        [HttpGet]
        public async Task<IActionResult> Search(string q, int page = 1)
        {
            try
            {
                if (page < 1) page = 1;

                var viewModel = await _categoryService.GetAllCategoriesAsync(page, q);
                
                ViewBag.SearchTerm = q;
                ViewBag.CurrentPage = page;
                
                return View("Index", viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred while searching categories. Please try again.";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX endpoint for real-time search
        [HttpGet]
        public async Task<IActionResult> QuickSearch(string term)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
                {
                    return Json(new { success = false, message = "Search term must be at least 2 characters long." });
                }

                var viewModel = await _categoryService.GetAllCategoriesAsync(1, term);
                var results = viewModel.Categories.Take(5).Select(c => new
                {
                    id = c.Id,
                    name = c.Name,
                    description = c.Description,
                    productCount = c.ProductCount
                });

                return Json(new { success = true, results });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred during search." });
            }
        }
    }
}

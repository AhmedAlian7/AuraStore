using E_Commerce.Business.Services.Implementation;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Category;
using E_Commerce.Business.ViewModels.PromoCode;
using E_Commerce.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]

    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string? searchTerm = null)
        {
            var models = await _categoryService.GetAllCategoriesAsync(page , searchTerm);

            return View(models);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var addCategory = new CategoryViewModel();

            return View("Add", addCategory);

        }

        [HttpPost]
        public async Task<IActionResult> Add(CategoryViewModel newCategory)
        {
            if (!ModelState.IsValid)
            {

                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View("Add", newCategory);
            }
            var add = await _categoryService.AddCategoryAsync(newCategory);
            if (add == false)
            {
                TempData["ErrorMessage"] = "Category already exists.";
                return RedirectToAction("Add");
            }
            else
            {
                TempData["SuccessMessage"] = "Category added successfully.";
                return RedirectToAction("Index");
            }
        }
    }
}

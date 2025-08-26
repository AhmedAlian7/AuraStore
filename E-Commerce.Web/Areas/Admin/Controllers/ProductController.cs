using Microsoft.AspNetCore.Mvc.Rendering;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly IUnitOfWork _unitIfWork;
        public ProductController(IProductService productService, IUnitOfWork unitOfWork)
        {
            _productService = productService;
            _unitIfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string search = null, int? category = null, string sortBy = null)
        {
            var products = await _productService.GetAllAsync(page, search, category, sortBy);
           
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
        public IActionResult Add()
        {
            var addProduct = new ProductAddViewModel
            {
                Categories = _unitIfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                })
            };

            return View("Add", addProduct);

        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductAddViewModel productResponse)
        {
            if (!ModelState.IsValid)
            {

                productResponse.Categories = _unitIfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View("Add", productResponse);
            }
            await _productService.AddProductAsync(productResponse);
            TempData["SuccessMessage"] = "Product added successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Update(int Id)
        {
            var product = await _unitIfWork.Products.GetByIdAsync(Id, "Category,ProductImages");
            if (product is null)
            {
                TempData["ErrorMessage"] = "Product not found.";
                return RedirectToAction("Index");
            }
            var updateModel = new ProductUpdateViewModel
            {
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                StockCount = product.StockCount,
                CategoryId = product.CategoryId,
                MainImageUrl = product.MainImageUrl,
                CategoriesList = _unitIfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }),
                CurrentAdditionalImages = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? []

            };
            return View(updateModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var existingProduct = await _unitIfWork.Products.GetByIdAsync(model.Id);
                if (existingProduct != null)
                {
                    model.MainImageUrl = existingProduct.MainImageUrl;
                    model.CurrentAdditionalImages = existingProduct.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>();
                }

                model.CategoriesList = _unitIfWork.Categories.GetAll().Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                });
                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View(model);
            }

            var product = new Product
            {
                Id = model.Id,
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DiscountPrice = model.DiscountPrice,
                StockCount = model.StockCount,
                CategoryId = model.CategoryId,
                MainImageUrl = model.MainImageUrl
            };

            // Handle Main Image Updates
            await _productService.HandleMainImageUpdate(product, model);

            // Handle Additional Images Updates
            await _productService.HandleAdditionalImagesUpdate(product, model);

            _unitIfWork.Products.Update(product);
            await _unitIfWork.SaveAsync();
            TempData["SuccessMessage"] = "Product updated successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);

            if (!result)
            {
                TempData["ErrorMessage"] = "Product not found.";
            }
            else
            {
                TempData["SuccessMessage"] = "Product deleted successfully.";
            }

            return RedirectToAction(nameof(Index));
        }

    }
}

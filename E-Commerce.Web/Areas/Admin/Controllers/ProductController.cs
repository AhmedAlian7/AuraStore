using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Repositories.Implementation;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
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
        public async Task<IActionResult> Index(int page = 1)
        {
            var products = await _productService.GetAllAsync(page);

            ViewBag.Categories = await _unitIfWork.Categories.GetAllAsync("");
            return View("Index", products);
        }

        [HttpGet]
        [Authorize(Roles = AppRoles.Admin)]
        public IActionResult Add()
        {
            var addProduct= new ProductAddViewModel
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
            return RedirectToAction("Index");
        }


    }
}

using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
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
    }
}

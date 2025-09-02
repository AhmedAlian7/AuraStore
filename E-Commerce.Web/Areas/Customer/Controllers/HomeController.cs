using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Home;
using E_Commerce.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Threading.Tasks;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;   
        private readonly IReviewService _reviewService;

        public HomeController(ILogger<HomeController> logger, IProductService productService
            , IReviewService reviewService)
        {
            _logger = logger;
            _productService = productService;
            _reviewService = reviewService;
        }

        public async Task<IActionResult> Index()
        {
            var trendingProducts = await _productService.GetBestSalesProductsAsync(4);
            var homeReviews = await _reviewService.GetLatestReviewsAsync(3);
            var vm = new HomePageViewModel
            {
                TrendingProducts = trendingProducts,
                HomeReviews = homeReviews
            };



            return View(vm);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

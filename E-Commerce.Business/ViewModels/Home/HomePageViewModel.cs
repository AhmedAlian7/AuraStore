using System.Collections.Generic;
using E_Commerce.Business.ViewModels.Product;

namespace E_Commerce.Business.ViewModels.Home
{
    public class HomePageViewModel
    {
        public IEnumerable<ProductViewModel> TrendingProducts { get; set; } = new List<ProductViewModel>();
        public IEnumerable<ReviewViewModel> HomeReviews { get; set; } = new List<ReviewViewModel>();
    }
}

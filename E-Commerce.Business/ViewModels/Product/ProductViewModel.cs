using E_Commerce.DataAccess.Entities;

namespace E_Commerce.Business.ViewModels.Product
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal EffectivePrice { get; set; }

        public bool InStock { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        public string MainImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
    }
}

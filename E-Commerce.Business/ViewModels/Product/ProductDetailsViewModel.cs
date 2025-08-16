
namespace E_Commerce.Business.ViewModels.Product
{
    public class ProductDetailsViewModel
    {
        // Basic Product Information
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal EffectivePrice { get; set; }
        public int StockCount { get; set; }
        public bool InStock { get; set; }

        // Category Information
        public string CategoryName { get; set; } = string.Empty;

        // Images
        public string MainImageUrl { get; set; } = string.Empty;
        public List<ProductImageViewModel> AdditionalImages { get; set; } = new();

        // Rating & Reviews Summary
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }

        // Reviews List
        public List<ReviewViewModel> Reviews { get; set; } = new();

        // Rating Breakdown for Summary Chart
        public RatingBredownViewModel RatingBreakdown { get; set; } = new();

        // Stock Status Display Properties
        public bool IsLowStock => InStock && StockCount <= 2;
        public string StockStatusText => InStock ? "In Stock" : "Out of Stock";
        public string StockStatusColor => InStock ? "green" : "red";
        public string LowStockWarning => IsLowStock ? $"Only {StockCount} left!" : string.Empty;

        // Price Display Properties
        public bool HasDiscount => DiscountPrice.HasValue && DiscountPrice < Price;
    }

    public class ProductImageViewModel
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }

    public class ReviewViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVerifiedPurchase { get; set; }

        // Formatted display properties
        public string FormattedDate => CreatedAt.ToString("MMM dd, yyyy");
        public string StarRating => new string('★', Rating) + new string('☆', 5 - Rating);
    }

    public class RatingBredownViewModel
    {
        public int FiveStarCount { get; set; }
        public int FourStarCount { get; set; }
        public int ThreeStarCount { get; set; }
        public int TwoStarCount { get; set; }
        public int OneStarCount { get; set; }
        public int TotalReviews { get; set; }

        // Percentage properties for bar chart display
        public double FiveStarPercentage => TotalReviews > 0 ? (FiveStarCount * 100.0) / TotalReviews : 0;
        public double FourStarPercentage => TotalReviews > 0 ? (FourStarCount * 100.0) / TotalReviews : 0;
        public double ThreeStarPercentage => TotalReviews > 0 ? (ThreeStarCount * 100.0) / TotalReviews : 0;
        public double TwoStarPercentage => TotalReviews > 0 ? (TwoStarCount * 100.0) / TotalReviews : 0;
        public double OneStarPercentage => TotalReviews > 0 ? (OneStarCount * 100.0) / TotalReviews : 0;
    }
}

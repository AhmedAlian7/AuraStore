namespace E_Commerce.Business.ViewModels.Wishlist
{
    public class WishlistItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ProductDescription { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public decimal EffectivePrice => DiscountPrice ?? Price;
        public string MainImageUrl { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public bool InStock { get; set; }
        public int StockCount { get; set; }
        public DateTime AddedAt { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}

namespace E_Commerce.Business.ViewModels.Dtos
{
    public class ProductUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockCount { get; set; }
        public int CategoryId { get; set; }
        public string? MainImageUrl { get; set; }
    }
}
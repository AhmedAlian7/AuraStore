namespace E_Commerce.Business.ViewModels.Product
{
    public class ProductFilterViewModel
    {
        public int Page { get; set; } = 1;
        public string? Search { get; set; }
        public int? CategoryId { get; set; }
        public string? SortBy { get; set; }
        public bool? InStock { get; set; }
    }

    public class ProductFilterResultViewModel
    {
        public PaginatedList<ProductViewModel> Products { get; set; } = null!;
        public int TotalProducts { get; set; }
        public int InStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }
}

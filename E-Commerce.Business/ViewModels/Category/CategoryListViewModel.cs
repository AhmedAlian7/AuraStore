using E_Commerce.Business.ViewModels;

namespace E_Commerce.Business.ViewModels.Category
{
    public class CategoryListViewModel
    {
        public PaginatedList<CategoryViewModel> Categories { get; set; } = new PaginatedList<CategoryViewModel>(new List<CategoryViewModel>(), 0, 1, 10);
        public string? SearchTerm { get; set; }
        public int TotalCategories { get; set; }
        public bool HasSearchResults => !string.IsNullOrWhiteSpace(SearchTerm);
        public bool IsEmpty => !Categories.Any();
    }
}

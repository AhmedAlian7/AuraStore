using E_Commerce.Business.ViewModels.Category;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryListViewModel> GetAllCategoriesAsync(int page = 1, string? searchTerm = null);
        Task<bool> CategoryExistsAsync(int categoryId);
    }
}

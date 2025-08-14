
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        //Task<IEnumerable<Product>> GetFeaturedProductsAsync();
        //Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm);
        Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<IEnumerable<Product>> GetProductsByRatingAsync(double minRating);
        Task<IEnumerable<Product>> GetLatestProductsAsync(int count);
        Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count);
    }
}

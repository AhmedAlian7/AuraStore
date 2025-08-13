
using E_Commerce.Business.ViewModels;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllAsync(int page = 1);

        //Task<ProductViewModel?> GetProductByIdAsync(int? id);

        //Task AddProductAsync(ProductViewModel productVm);

        //Task UpdateProductAsync(UpdateProductViewModel updateProductVM);

        //Task DeleteProductAsync(int? id);

        //Task<IEnumerable<ProductViewModel>> GetBestSalesProductsAsync(int count = 8);

        //Task<IEnumerable<ProductViewModel>> GetRecentProductsAsync(int count = 8);

        //Task<IEnumerable<ProductViewModel>> GetProductsByCategoryIdAsync(int typeId);

        //Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string query);
    }
}

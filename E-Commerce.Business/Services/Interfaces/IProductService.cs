using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllAsync(int page = 1, string search = null, int? categoryId = null, string sortBy = null);

        //Task<ProductViewModel?> GetProductByIdAsync(int? id);

        Task AddProductAsync(ProductAddViewModel product);

        //Task UpdateProductAsync(UpdateProductViewModel updateProductVM);

        Task<IEnumerable<ProductViewModel>> GetBestSalesProductsAsync(int count = 8);

        Task<IEnumerable<ProductViewModel>> GetRecentProductsAsync(int count = 8);

        Task<IEnumerable<ProductViewModel>> GetProductsByCategoryIdAsync(int typeId);


        Task HandleMainImageUpdate(Product product, ProductUpdateViewModel model);
        Task HandleAdditionalImagesUpdate(Product product, ProductUpdateViewModel model);

        Task<ProductDetailsViewModel> GetProductDetailsAsync(int id);
        Task<bool> DeleteProductAsync(int id);

        Task AddReviewAsync(int productId, string userId, int rating, string comment);

        //Task<bool> AddToCart(int productId, int quantity = 1);

    }

}

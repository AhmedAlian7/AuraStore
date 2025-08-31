using E_Commerce.Business.Services.Implementation;
using E_Commerce.Business.ViewModels.Dtos;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductViewModel>> GetAllAsync(int page = 1, string search = null, int? categoryId = null, string sortBy = null);

        // New enhanced filtering method
        Task<ProductFilterResultViewModel> GetFilteredProductsAsync(ProductFilterViewModel filter);

        Task AddProductAsync(ProductAddViewModel product);

        Task<ProductPostDto> AddProductAsync(ProductPostDto dto);
        Task<ProductDto> UpdateProductAsync(ProductUpdateDto dto);
        Task<IEnumerable<ProductViewModel>> GetBestSalesProductsAsync(int count = 8);

        Task<IEnumerable<ProductViewModel>> GetRecentProductsAsync(int count = 8);

        Task HandleMainImageUpdate(Product product, ProductUpdateViewModel model);
        Task HandleAdditionalImagesUpdate(Product product, ProductUpdateViewModel model);

        Task<ProductDetailsViewModel> GetProductDetailsAsync(int id);
        Task<bool> DeleteProductAsync(int id);

        Task AddReviewAsync(int productId, string userId, int rating, string comment);


        Task<IEnumerable<ProductDto>> GetAllProductsDtoAsync();
        Task<ProductDto> GetProductDto(int Id);
        Task<bool> HasNotificationRequestAsync(int productId, string userId);
        Task<bool> UpdateProductAsync(ProductUpdateViewModel model);

    }

}

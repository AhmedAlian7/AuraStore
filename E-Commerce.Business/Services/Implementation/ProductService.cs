
using AutoMapper;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.VisualBasic;
using mvcFirstApp.Services;
using E_Commerce.DataAccess.Constants;

namespace E_Commerce.Business.Services.Implementation
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly FileUploadService _uploadService;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, FileUploadService uploadService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uploadService = uploadService;
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllAsync(int page = 1)
        {
            var products = await _unitOfWork.Products.GetAllAsync(page, "Category");

            var productsVM = products.Select( p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,

                Price = p.Price,
                DiscountPrice = p.DiscountPrice,
                EffectivePrice = p.DiscountPrice ?? p.Price,

                InStock = p.StockCount > 0,
                AverageRating = (p.Reviews != null && p.Reviews.Any()) ? p.Reviews.Average(r => r.Rating) : 0,
                ReviewCount = p.Reviews?.Count ?? 0,

                MainImageUrl = p.MainImageUrl,
                CategoryName = p.Category?.Name ?? string.Empty
            });

            return PaginatedList<ProductViewModel>.Create(productsVM, page, Numbers.DefaultPageSize);
        }

        public async Task<IEnumerable<ProductViewModel>> GetRecentProductsAsync(int count = 8)
        {

            var products = await _unitOfWork.Products.GetLatestProductsAsync(count);

            var ProductsVM = _mapper.Map<IEnumerable<ProductViewModel>>(products);


            return ProductsVM;

        }

        public async Task<IEnumerable<ProductViewModel>> GetBestSalesProductsAsync(int count = 8)
        {

            var products = await _unitOfWork.Products.GetTopSellingProductsAsync(count);

            var ProductsVM = _mapper.Map<IEnumerable<ProductViewModel>>(products);


            return ProductsVM;

        }

        public async Task<IEnumerable<ProductViewModel>> GetProductsByCategoryIdAsync(int CategoryId)
        {

            var products = await _unitOfWork.Products.GetProductsByCategoryAsync(CategoryId);

            var ProductsVM = _mapper.Map<IEnumerable<ProductViewModel>>(products);

            return ProductsVM;


        }

        public async Task AddProductAsync(ProductAddViewModel model)
        {
            var productEntity = new Product
            {
                Name = model.Name,
                Description = model.Description,
                Price = model.Price,
                DiscountPrice = model.DiscountPrice,
                StockCount = model.StockCount,
                CategoryId = model.CategoryId,
                MainImageUrl = model.MainImageUrl ?? string.Empty,
                ProductImages = new List<ProductImage>()
            };


            if (model.MainImageFile != null)
            {
                try
                {
                    productEntity.MainImageUrl = await _uploadService.UploadAsync(model.MainImageFile, "products");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error uploading main image: " + ex.Message);
                }

            }
            productEntity.ProductImages ??= new List<ProductImage>();

            if (model.AdditionalImages != null && model.AdditionalImages.Any())
            {
                int displayOrder = 1;

                foreach (var image in model.AdditionalImages)
                {
                    if (image != null && image.Length > 0)
                    {
                        try
                        {
                            var imageUrl = await _uploadService.UploadAsync(image, "products");
                            var additionalImage = new ProductImage
                            {
                                // <<== error : if force to set Id, it will be set by EF Core how to fix
                                DisplayOrder = displayOrder++,
                                ImageUrl = imageUrl,
                                IsMain = false,
                                ProductId = productEntity.Id // This will be set after the product is saved
                            };
                            productEntity.ProductImages.Add(additionalImage);
                        }
                        catch (Exception ex)
                        {
                            throw new InvalidOperationException($"Error uploading additional image {displayOrder}: {ex.Message}");
                        }
                    }
                }
            }

            try
            {
                await _unitOfWork.Products.AddAsync(productEntity);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error saving product to database: " + ex.Message);
            }
        }
    }
}

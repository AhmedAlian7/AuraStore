using AutoMapper;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Dtos;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using mvcFirstApp.Services;

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

            var productsVM = products.Select(p => new ProductViewModel
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

        public async Task<IEnumerable<ProductViewModel>> GetAllAsync(int page = 1, string search = null, int? categoryId = null, string sortBy = null)
        {
            // Get all products with includes
            var productsQuery = _unitOfWork.Products.GetAllQueryable("Category,Reviews");

            if (!string.IsNullOrWhiteSpace(search))
            {
                productsQuery = productsQuery.Where(p => p.Name.Contains(search) || (p.Description != null && p.Description.Contains(search)));
            }
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId.Value);
            }
            // Sorting
            productsQuery = sortBy switch
            {
                "name" => productsQuery.OrderBy(p => p.Name),
                "price_asc" => productsQuery.OrderBy(p => p.DiscountPrice ?? p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.DiscountPrice ?? p.Price),
                "rating" => productsQuery.OrderByDescending(p => p.Reviews.Any() ? p.Reviews.Average(r => r.Rating) : 0),
                "newest" => productsQuery.OrderByDescending(p => p.CreatedAt),
                _ => productsQuery.OrderBy(p => p.Id)
            };

            int pageSize = Numbers.DefaultPageSize;
            var products = productsQuery.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            var productsVM = products.Select(p => new ProductViewModel
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

            return PaginatedList<ProductViewModel>.Create(productsVM, page, pageSize);
        }

        public async Task<IEnumerable<ProductViewModel>> GetRecentProductsAsync(int count = 8)
        {

            var products = await _unitOfWork.Products.GetLatestProductsAsync(count);

            var ProductsVM = _mapper.Map<IEnumerable<ProductViewModel>>(products);


            return ProductsVM;

        }

        public async Task<IEnumerable<ProductViewModel>> GetBestSalesProductsAsync(int count = 4)
        {

            var products = await _unitOfWork.Products.GetTopSellingProductsAsync(count);

            var productsVM = products.Select(p => new ProductViewModel
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

            return productsVM;

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
            productEntity.ProductImages ??= [];

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


        public async Task HandleMainImageUpdate(Product product, ProductUpdateViewModel model)
        {
            // If user wants to remove the current main image
            if (model.RemoveMainImage && !string.IsNullOrEmpty(product.MainImageUrl))
            {
                // Delete the physical file
                _uploadService.DeleteFile(product.MainImageUrl);
                product.MainImageUrl = null;
            }

            // If user uploaded a new main image
            if (model.MainImage != null && model.MainImage.Length > 0)
            {
                // Delete the old main image if it exists
                if (!string.IsNullOrEmpty(product.MainImageUrl))
                {
                    _uploadService.DeleteFile(product.MainImageUrl);
                }

                // Upload the new main image
                var newMainImageUrl = await _uploadService.UploadAsync(model.MainImage, "products");
                product.MainImageUrl = newMainImageUrl;
            }
        }

        public async Task HandleAdditionalImagesUpdate(Product product, ProductUpdateViewModel model)
        {
            // Get current additional images
            var currentAdditionalImages = product.ProductImages?.ToList() ?? [];

            // Handle removal of existing additional images
            if (model.RemoveAdditionalImages != null && model.CurrentAdditionalImages != null)
            {
                for (int i = 0; i < model.RemoveAdditionalImages.Count && i < model.CurrentAdditionalImages.Count; i++)
                {
                    if (model.RemoveAdditionalImages[i]) // If marked for removal
                    {
                        var imageUrlToRemove = model.CurrentAdditionalImages[i];

                        // Find and remove from database
                        var imageToRemove = currentAdditionalImages.FirstOrDefault(img => img.ImageUrl == imageUrlToRemove);
                        if (imageToRemove != null)
                        {
                            currentAdditionalImages.Remove(imageToRemove);
                            await _unitOfWork.ProductImages.DeleteAsync(imageToRemove.Id);

                            // Delete the physical file
                            _uploadService.DeleteFile(imageUrlToRemove);
                        }
                    }
                }
            }

            // Handle new additional images upload
            if (model.AdditionalImages != null && model.AdditionalImages.Any())
            {
                foreach (var newImage in model.AdditionalImages)
                {
                    if (newImage != null && newImage.Length > 0)
                    {
                        var newImageUrl = await _uploadService.UploadAsync(newImage, "products");
                        currentAdditionalImages.Add(new ProductImage
                        {
                            ProductId = product.Id,
                            ImageUrl = newImageUrl
                        });
                    }
                }
            }

            // Update the product's additional images collection
            product.ProductImages = currentAdditionalImages;
        }

        public async Task<ProductDetailsViewModel> GetProductDetailsAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id, "Category, Reviews.User, ProductImages");
            if (product == null)
            {
                throw new KeyNotFoundException($"Product with ID {id} not found.");
            }
            var reviews = product.Reviews?.ToList() ?? new List<Review>();
            var productDetailsVM = new ProductDetailsViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                EffectivePrice = product.EffectivePrice,
                StockCount = product.StockCount,
                InStock = product.InStock,
                CategoryName = product.Category?.Name ?? string.Empty,
                MainImageUrl = product.MainImageUrl,
                AdditionalImages = product.ProductImages.Select(img => new ProductImageViewModel
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    AltText = $"{product.Name} - Image {img.Id}"
                }).ToList(),
                AverageRating = reviews.Any() ? reviews.Average(r => r.Rating) : 0,
                ReviewCount = reviews.Count,
                Reviews = reviews.OrderByDescending(r => r.CreatedAt).Select(r => new ReviewViewModel
                {
                    Id = r.Id,
                    UserName = r.User?.Email?.Split('@')[0] ?? "Anonymous",
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    IsVerifiedPurchase = r.IsVerifiedPurchase
                }).ToList(),
                RatingBreakdown = new RatingBredownViewModel
                {
                    FiveStarCount = reviews.Count(r => r.Rating == 5),
                    FourStarCount = reviews.Count(r => r.Rating == 4),
                    ThreeStarCount = reviews.Count(r => r.Rating == 3),
                    TwoStarCount = reviews.Count(r => r.Rating == 2),
                    OneStarCount = reviews.Count(r => r.Rating == 1),
                    TotalReviews = reviews.Count
                }
            };
            return productDetailsVM;
        }

        public async Task<bool> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(id);
            if (product == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(product.MainImageUrl))
            {
                _uploadService.DeleteFile(product.MainImageUrl);
            }
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                foreach (var img in product.ProductImages)
                {
                    _uploadService.DeleteFile(img.ImageUrl);
                }
            }

            await _unitOfWork.Products.DeleteAsync(id);

            await _unitOfWork.SaveAsync();
            return true;
        }

        public async Task AddReviewAsync(int productId, string userId, int rating, string comment)
        {
            // Use repository method to check if user purchased the product
            bool hasPurchased = await _unitOfWork.OrderItems.HasUserPurchasedProductAsync(userId, productId);

            var review = new Review
            {
                ProductId = productId,
                UserId = userId,
                Rating = rating,
                Comment = comment,
                IsVerifiedPurchase = hasPurchased,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveAsync();
        }

        public async Task<ProductDto> GetProductDto(int Id)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(Id, "Category,Reviews");


            if (product == null) return null;
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.DiscountPrice ?? product.Price,
                DiscountPrice = product.DiscountPrice,
                StockCount = product.StockCount,
                ImageUrl = product.MainImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category.Name,
                Rating = product.Reviews != null && product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,

            };
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsDtoAsync()
        {
            var products = await _unitOfWork.Products.GetAllAsync("Category,Reviews");

            var productsDto = products.Select(p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.DiscountPrice ?? p.Price,
                DiscountPrice = p.DiscountPrice,
                StockCount = p.StockCount,
                ImageUrl = p.MainImageUrl,
                CategoryId = p.CategoryId,
                CategoryName = p.Category?.Name ?? string.Empty,
                Rating = (p.Reviews != null && p.Reviews.Any()) ? p.Reviews.Average(r => r.Rating) : 0
            });

            return productsDto;
        }

        public async Task<ProductPostDto> AddProductAsync(ProductPostDto dto)
        {

            var productEntity = new Product
            {
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                DiscountPrice = dto.DiscountPrice,
                StockCount = dto.StockCount,
                CategoryId = dto.CategoryId,
                MainImageUrl = dto.MainImageUrl ?? string.Empty,
            };


            if (dto.MainImageUrl != null)
            {
                try
                {
                    productEntity.MainImageUrl = await _uploadService.UploadAsync(dto.MainImageUrl, "products");
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Error uploading main image: " + ex.Message);
                }

            }
            productEntity.ProductImages ??= [];

            try
            {
                await _unitOfWork.Products.AddAsync(productEntity);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error saving product to database: " + ex.Message);
            }

            return dto;

        }
        public async Task<ProductDto> UpdateProductAsync(ProductUpdateDto dto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(dto.Id);
            if (product == null) return null;

            product.Name = dto.Name;
            product.Description = dto.Description;
            product.Price = dto.Price;
            product.DiscountPrice = dto.DiscountPrice;
            product.StockCount = dto.StockCount;
            product.CategoryId = dto.CategoryId;
            if (!string.IsNullOrEmpty(dto.MainImageUrl))
            {
                product.MainImageUrl = dto.MainImageUrl;
            }

            _unitOfWork.Products.Update(product);
            await _unitOfWork.SaveAsync();

            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.DiscountPrice ?? product.Price,
                DiscountPrice = product.DiscountPrice,
                StockCount = product.StockCount,
                ImageUrl = product.MainImageUrl,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                Rating = product.Reviews != null && product.Reviews.Any() ? product.Reviews.Average(r => r.Rating) : 0,
                CreatedAt = product.CreatedAt
            };
        }


    }
}

using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Wishlist;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Repositories.Interfaces;
using AutoMapper;

namespace E_Commerce.Business.Services.Implementation
{
    public class WishlistService : IWishlistService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICartService _cartService;
        private readonly IMapper _mapper;

        public WishlistService(IUnitOfWork unitOfWork, ICartService cartService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _cartService = cartService;
            _mapper = mapper;
        }

        public async Task<bool> AddToWishlistAsync(string userId, int productId)
        {
            try
            {
                // Validate product exists
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                    throw new ArgumentException("Product not found.");

                // Check if already in wishlist
                var existingItem = await _unitOfWork.WishlistItems.GetByUserAndProductAsync(userId, productId);
                if (existingItem != null)
                    return false; // Already in wishlist

                // Add to wishlist
                await _unitOfWork.WishlistItems.AddToWishlistAsync(userId, productId);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> RemoveFromWishlistAsync(string userId, int productId)
        {
            try
            {
                // Remove from wishlist
                await _unitOfWork.WishlistItems.RemoveFromWishlistAsync(userId, productId);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<WishlistViewModel> GetUserWishlistAsync(string userId, int page = 1)
        {
            try
            {
                // Get wishlist items with product details
                var wishlistItems = await _unitOfWork.WishlistItems.GetUserWishlistWithProductsAsync(userId);
                
                // Apply pagination
                var pageSize = Numbers.DefaultPageSize;
                var totalCount = wishlistItems.Count();
                var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);
                
                var pagedItems = wishlistItems
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Map to ViewModels
                var wishlistItemViewModels = pagedItems.Select(item => new WishlistItemViewModel
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ProductDescription = item.Product.Description ?? string.Empty,
                    Price = item.Product.Price,
                    DiscountPrice = item.Product.DiscountPrice,
                    MainImageUrl = item.Product.MainImageUrl,
                    CategoryName = item.Product.Category?.Name ?? string.Empty,
                    InStock = item.Product.InStock,
                    StockCount = item.Product.StockCount,
                    AddedAt = item.AddedAt,
                    AverageRating = item.Product.AverageRating,
                    ReviewCount = item.Product.ReviewCount
                }).ToList();

                return new WishlistViewModel
                {
                    WishlistItems = wishlistItemViewModels,
                    TotalCount = totalCount,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalPages = totalPages
                };
            }
            catch (Exception)
            {
                return new WishlistViewModel
                {
                    WishlistItems = new List<WishlistItemViewModel>(),
                    TotalCount = 0,
                    CurrentPage = page,
                    PageSize = Numbers.DefaultPageSize,
                    TotalPages = 0
                };
            }
        }

        public async Task<bool> IsInWishlistAsync(string userId, int productId)
        {
            try
            {
                return await _unitOfWork.WishlistItems.IsInWishlistAsync(userId, productId);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<int> GetWishlistCountAsync(string userId)
        {
            try
            {
                return await _unitOfWork.WishlistItems.GetWishlistCountAsync(userId);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<bool> MoveToCartAsync(string userId, int productId)
        {
            try
            {
                // Validate product exists
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null)
                    throw new ArgumentException("Product not found.");

                // Check if product is in stock
                if (!product.InStock)
                    throw new InvalidOperationException("Product is out of stock.");

                // Add to cart
                await _cartService.AddToCartAsync(userId, productId, 1);

                // Remove from wishlist
                await _unitOfWork.WishlistItems.RemoveFromWishlistAsync(userId, productId);
                await _unitOfWork.SaveAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<WishlistItemViewModel?> GetWishlistItemAsync(string userId, int productId)
        {
            try
            {
                var wishlistItem = await _unitOfWork.WishlistItems.GetByUserAndProductAsync(userId, productId);
                if (wishlistItem == null)
                    return null;

                return new WishlistItemViewModel
                {
                    Id = wishlistItem.Id,
                    ProductId = wishlistItem.ProductId,
                    ProductName = wishlistItem.Product.Name,
                    ProductDescription = wishlistItem.Product.Description ?? string.Empty,
                    Price = wishlistItem.Product.Price,
                    DiscountPrice = wishlistItem.Product.DiscountPrice,
                    MainImageUrl = wishlistItem.Product.MainImageUrl,
                    CategoryName = wishlistItem.Product.Category?.Name ?? string.Empty,
                    InStock = wishlistItem.Product.InStock,
                    StockCount = wishlistItem.Product.StockCount,
                    AddedAt = wishlistItem.AddedAt,
                    AverageRating = wishlistItem.Product.AverageRating,
                    ReviewCount = wishlistItem.Product.ReviewCount
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}

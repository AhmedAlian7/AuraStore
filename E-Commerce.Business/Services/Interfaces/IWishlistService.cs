using E_Commerce.Business.ViewModels.Wishlist;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IWishlistService
    {
        Task<bool> AddToWishlistAsync(string userId, int productId);
        Task<bool> RemoveFromWishlistAsync(string userId, int productId);
        Task<WishlistViewModel> GetUserWishlistAsync(string userId, int page = 1);
        Task<bool> IsInWishlistAsync(string userId, int productId);
        Task<int> GetWishlistCountAsync(string userId);
        Task<bool> MoveToCartAsync(string userId, int productId);
        Task<WishlistItemViewModel?> GetWishlistItemAsync(string userId, int productId);
    }
}

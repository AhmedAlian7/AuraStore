using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IWishlistItemRepository : IRepository<WishlistItem>
    {
        Task<IEnumerable<WishlistItem>> GetUserWishlistAsync(string userId);
        Task<WishlistItem?> GetByUserAndProductAsync(string userId, int productId);
        Task AddToWishlistAsync(string userId, int productId);
        Task RemoveFromWishlistAsync(string userId, int productId);
        Task<bool> IsInWishlistAsync(string userId, int productId);
        Task<int> GetWishlistCountAsync(string userId);
        Task<IEnumerable<WishlistItem>> GetUserWishlistWithProductsAsync(string userId);
    }
}

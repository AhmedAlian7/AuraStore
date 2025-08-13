
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetByUserIdAsync(string userId);
        Task<Cart?> GetCartWithItemsAsync(string userId);
        Task<Cart?> GetCartWithItemsAndProductsAsync(string userId);
        Task ClearCartAsync(string userId);
        Task<int> GetCartItemCountAsync(string userId);
        Task<decimal> GetCartTotalAsync(string userId);
    }
}

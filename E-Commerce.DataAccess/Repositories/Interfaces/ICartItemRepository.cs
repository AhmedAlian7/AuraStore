
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task<IEnumerable<CartItem>> GetByCartIdAsync(int cartId);
        Task<CartItem?> GetByCartAndProductAsync(int cartId, int productId);
        Task UpdateQuantityAsync(int cartItemId, int quantity);
    }
}

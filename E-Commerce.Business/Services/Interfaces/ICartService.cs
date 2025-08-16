using E_Commerce.DataAccess.Entities;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int productId, int quantity = 1);
        Task<Cart> GetUserCartAsync(string userId);
    }

}

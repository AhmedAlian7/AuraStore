using E_Commerce.Business.ViewModels.Cart;
using Stripe.Checkout;
namespace E_Commerce.Business.Services.Interfaces
{
    public interface ICartService
    {
        Task AddToCartAsync(string userId, int productId, int quantity = 1);
        Task<CartViewModel> GetUserCartAsync(string userId);

        Task<CartSummaryDto> GetCartSummaryAsync();
        Task<UpdateQuantityResult> UpdateQuantityAsync(int cartItemId, int quantity);
        Task DeleteItemAsync(int cartItemId);
    }
}

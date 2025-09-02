using E_Commerce.Business.Services.Interfaces;

namespace E_Commerce.Business.Services.Implementation
{
    public class BadgeCountService : IBadgeCountService
    {
        private readonly ICartService _cartService;
        private readonly IWishlistService _wishlistService;

        public BadgeCountService(ICartService cartService, IWishlistService wishlistService)
        {
            _cartService = cartService;
            _wishlistService = wishlistService;
        }

        public async Task<(int CartCount, int WishlistCount)> GetBadgeCountsAsync(string userId)
        {
            // Use Task.WhenAll for parallel execution of both count operations
            var cartTask = _cartService.GetCartSummaryAsync();
            var wishlistTask = _wishlistService.GetWishlistCountAsync(userId);

            await Task.WhenAll(cartTask, wishlistTask);

            var cartSummary = await cartTask;
            var wishlistCount = await wishlistTask;

            return (cartSummary?.TotalItems ?? 0, wishlistCount);
        }
    }
}

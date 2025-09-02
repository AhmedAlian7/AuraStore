namespace E_Commerce.Business.Services.Interfaces
{
    public interface IBadgeCountService
    {
        Task<(int CartCount, int WishlistCount)> GetBadgeCountsAsync(string userId);
    }
}

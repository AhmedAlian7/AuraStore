using E_Commerce.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetByProductIdAsync(int productId);
        Task<IEnumerable<Review>> GetByUserIdAsync(string userId);
        Task<bool> HasUserPurchasedProductAsync(string userId, int productId);
        Task<IEnumerable<Review>> GetLatestReviewsAsync(int count); // Added for home page
    }
}

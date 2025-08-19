using System.Collections.Generic;
using System.Threading.Tasks;
using E_Commerce.Business.ViewModels.Product;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewViewModel>> GetLatestReviewsAsync(int count);
    }
}

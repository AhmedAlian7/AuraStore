
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IProductImageRepository : IRepository<ProductImage>
    {
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId);
        Task<ProductImage?> GetMainImageAsync(int productId);
    }
}

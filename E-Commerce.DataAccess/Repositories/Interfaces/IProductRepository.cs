using System.Linq;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IProductRepository : IRepository<Product>
    {
        Task<IEnumerable<Product>> GetLatestProductsAsync(int count);
        Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count);
        IQueryable<Product> GetAllQueryable(string includes = "");
    }
}

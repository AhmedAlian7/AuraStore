
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<bool> IsUniqueAsync(string categoryName, int? excludeId = null);
        Task<int> GetProductCountByCategoryAsync(int categoryId);

        IEnumerable<Category> GetAll();
    }
}

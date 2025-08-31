
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<int> GetProductCountByCategoryAsync(int categoryId);

        IEnumerable<Category> GetAll();
        IQueryable<Category> GetAllQueryable();
    }
}

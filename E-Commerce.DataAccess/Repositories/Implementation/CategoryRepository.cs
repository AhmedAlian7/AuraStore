using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<int> GetProductCountByCategoryAsync(int categoryId)
        {
            return await _context.Products.CountAsync(p => p.CategoryId == categoryId);
        }

        public async Task<bool> IsUniqueAsync(string categoryName, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Name == categoryName);
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }
            return !await query.AnyAsync();
        }
    }
}

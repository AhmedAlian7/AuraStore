using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class ProductImageRepository : Repository<ProductImage>, IProductImageRepository
    {
        public ProductImageRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(int productId)
        {
            return await _dbSet
                .Where(pi => pi.ProductId == productId)
                .OrderBy(pi => pi.DisplayOrder)
                .ToListAsync();
        }

        public async Task<ProductImage?> GetMainImageAsync(int productId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(pi => pi.ProductId == productId && pi.IsMain);
        }

    }
}

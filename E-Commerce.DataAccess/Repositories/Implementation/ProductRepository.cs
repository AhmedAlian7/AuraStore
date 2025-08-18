using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

        public IQueryable<Product> GetAllQueryable(string includes = "")
        {
            var query = _context.Products.AsQueryable();
            if (!string.IsNullOrEmpty(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }
            return query;
        }

        public async Task<IEnumerable<Product>> GetTopSellingProductsAsync(int count)
        {
            var topSellingProducts = await _context.OrderItems
                //.Include( oi => oi.Product) .Include( oi => oi.Order)
                .Where(oi => oi.Order.OrderStatus == OrderStatus.Delivered)
                .GroupBy(oi => oi.Product)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Select(g => g.Key) // g.Key is the Product
                .Take(count)
                .ToListAsync();

            return topSellingProducts;
        }

        public async Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {

            var ProductsInRange = await _context.Products
                .Where(p => p.EffectivePrice >= minPrice && p.EffectivePrice <= maxPrice)
                .OrderByDescending(p => p.EffectivePrice)
                .ToListAsync(); 
            
            return ProductsInRange;

        }

        public async Task<IEnumerable<Product>> GetProductsByRatingAsync(double minRating)
        {
           
            var productsByRating = await _context.Products
                .Where(p => p.AverageRating >= minRating)
                .OrderByDescending(p => p.AverageRating)
                .ToListAsync();

            return productsByRating; 

        }

        public async Task<IEnumerable<Product>> GetLatestProductsAsync(int count)
        {

            var LatestProducts = await _context.Products
                .OrderByDescending (p => p.CreatedAt)
                .Take (count)
                .ToListAsync();

            return LatestProducts;
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
             return await _context.Products.Where(p => p.CategoryId == categoryId).ToListAsync();
        }
    }
}

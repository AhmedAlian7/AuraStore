using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(AppDbContext context) : base(context) { }

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
    }
}

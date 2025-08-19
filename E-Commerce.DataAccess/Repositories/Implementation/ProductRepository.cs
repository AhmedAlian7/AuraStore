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
                .Where(oi => oi.Order.OrderStatus == OrderStatus.PendingPayment || oi.Order.OrderStatus == OrderStatus.Paid || oi.Order.OrderStatus == OrderStatus.Shipped || oi.Order.OrderStatus == OrderStatus.Delivered)
                .GroupBy(oi => oi.Product)
                .OrderByDescending(g => g.Sum(oi => oi.Quantity))
                .Select(g => g.Key) // g.Key is the Product
                .Take(count)
                .ToListAsync();

            return topSellingProducts;
        }

        public async Task<IEnumerable<Product>> GetLatestProductsAsync(int count)
        {

            var LatestProducts = await _context.Products
                .OrderByDescending (p => p.CreatedAt)
                .Take (count)
                .ToListAsync();

            return LatestProducts;
        }

    }
}

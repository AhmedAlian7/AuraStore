using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            CartItems = new CartItemRepository(_context);
            Carts = new CartRepository(_context);
            Categories = new CategoryRepository(_context);
            Products = new ProductRepository(_context);
            OrderItems = new OrderItemRepository(_context);
            Orders = new OrderRepository(_context);
            Reviews = new ReviewRepository(_context);
            ProductImages = new ProductImageRepository(_context);
            PromoCodes = new PromoCodeRepository(_context);
        }

        public ICartItemRepository CartItems { get; private set; }
        public ICartRepository Carts { get; private set; }
        public ICategoryRepository Categories { get; private set; }
        public IProductRepository Products { get; private set; }
        public IOrderItemRepository OrderItems { get; private set; }
        public IOrderRepository Orders { get; private set; }
        public IReviewRepository Reviews { get; private set; }
        public IProductImageRepository ProductImages { get; private set; }
        public IPromoCodeRepository PromoCodes { get; private set; }


        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

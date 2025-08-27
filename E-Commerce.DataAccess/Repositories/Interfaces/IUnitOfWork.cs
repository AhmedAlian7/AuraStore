
namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        ICartItemRepository CartItems { get; }
        ICartRepository Carts { get; }
        ICategoryRepository Categories { get; }
        IProductRepository Products { get; }
        IOrderItemRepository OrderItems { get; }
        IOrderRepository Orders { get; }
        IReviewRepository Reviews { get; }
        IProductImageRepository ProductImages { get; }
        IPromoCodeRepository PromoCodes { get; }
        IWishlistItemRepository WishlistItems { get; }

        Task<int> SaveAsync();
        //IRepository<TEntity> Repository<TEntity>() where TEntity : class;

    }
}

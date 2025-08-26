
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Repositories.Interfaces
{
    public interface IPromoCodeRepository : IRepository<PromoCode>
    {
        Task<PromoCode?> GetByCodeAsync(string code);
    }
}

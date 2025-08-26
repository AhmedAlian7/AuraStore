using E_Commerce.DataAccess.Data;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.DataAccess.Repositories.Implementation
{
    public class PromoCodeRepository : Repository<PromoCode>, IPromoCodeRepository
    {
        public PromoCodeRepository(AppDbContext context) : base(context)
        {
        }
        public async Task<PromoCode?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(pc => pc.Code == code);
        }
    }
}

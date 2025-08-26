using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Customer;
using E_Commerce.Business.ViewModels.PromoCode;
using E_Commerce.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IPromoCodeService
    {

        //Task<PromoCode> GetPromoCodeByCodeAsync(string code);

        Task<PaginatedList<PromoCodeViewModel>> GetAllAsync(int page);
        Task<bool> AddCodeAsync(PromoCodeViewModel vm);
    }
}

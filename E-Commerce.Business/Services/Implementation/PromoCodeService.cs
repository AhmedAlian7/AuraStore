using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.PromoCode;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.Web.WebPages;
namespace E_Commerce.Business.Services.Implementation
{

   
    public class PromoCodeService : IPromoCodeService
    {
        private readonly IUnitOfWork _unitOfWork;


        public PromoCodeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedList<PromoCodeViewModel>> GetAllAsync(int page)
        {
            var codes = await _unitOfWork.PromoCodes.GetAllAsync(page);

            var models = codes.Select(c => new PromoCodeViewModel
            {
                Code = c.Code,
                Description = c.Description,
                DiscountType = c.DiscountType,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                DiscountValue = c.DiscountValue,
                UsageLimit = c.UsageLimit,
                
            });


            return PaginatedList<PromoCodeViewModel>.Create(models,page,Numbers.DefaultPageSize);
        }

        public async Task<bool> AddCodeAsync (PromoCodeViewModel vm)
        {
            var code = new PromoCode
            {
                Code = vm.Code,
                Description = vm.Description,
                StartDate = vm.StartDate,
                EndDate = vm.EndDate,
                DiscountType = vm.DiscountType,
                UsageLimit = vm.UsageLimit,
                DiscountValue = vm.DiscountValue,
                IsActive = true,
            };
            
            await _unitOfWork.PromoCodes.AddAsync(code);
            await _unitOfWork.SaveAsync();

            return true;
        }

    }
}

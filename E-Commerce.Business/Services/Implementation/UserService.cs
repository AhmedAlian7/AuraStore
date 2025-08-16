
using AutoMapper;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Customer;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using mvcFirstApp.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce.Business.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<ApplicationUser> _userManager;
        
        public UserService(IUnitOfWork unitOfWork, UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }




        public async Task<IEnumerable<CustomerViewModel>> GetAllAsync(int page)
        {
            var users = _userManager.Users.ToList();

            var tasks = users.Select(async x => new CustomerViewModel
            {
                Id = x.Id.Substring(0,5),
                Email = x.Email,
                IsActive = x.IsActive,
                CreateAt = x.CreatedAt,
                //OrdersCount = await _unitOfWork.Orders.GetOrderCountByUserIdAsync(x.Id)
            });

            var models = await Task.WhenAll(tasks);

            return PaginatedList<CustomerViewModel>.Create(models, page, Numbers.DefaultPageSize);
        }

    }
}

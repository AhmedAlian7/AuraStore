
using AutoMapper;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Customer;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
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




        public async Task<PaginatedList<CustomerViewModel>> GetAllAsync(int page)
        {
            var users = _userManager.Users.Where(o => !o.IsDeleted)
                .ToList();
            var models = new List<CustomerViewModel>();

            foreach (var x in users)
            {
                var role = (await _userManager.GetRolesAsync(x)).FirstOrDefault();

                
                models.Add(new CustomerViewModel
                {
                    Id = x.Id,
                    Email = x.Email,
                    IsActive = x.IsActive,
                    CreateAt = x.CreatedAt,
                    Role = role
                });
            }

            var orderdModels = models.OrderByDescending(o => o.Role == "Admin");

            return PaginatedList<CustomerViewModel>.Create(orderdModels, page, Numbers.DefaultPageSize);
        }

        public async Task<ProfileViewModel> ShowProfile(string id)
        {
            var user = await _userManager
                .Users
                .Where(o => !o.IsDeleted)
                .Include(o => o.Orders)
                .FirstOrDefaultAsync(u => u.Id == id);


            var model = new ProfileViewModel
            {
                email = user.Email,
                OrdersCount = user.Orders?.Count() ?? 0,
            };
            
            return model;
        }

        public async Task<bool> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null || user.IsDeleted)
            {
                return false;
            }

            var result = user.IsDeleted = true;
            await _userManager.UpdateAsync(user);

            return result;
        }

        public async Task<bool> ChangeStatus(string id, string status)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return false;
            }
            if (status == "Active")
            {
                user.IsActive = true;
            }
            else
            {
                user.IsActive = false;
            }

            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

    }
}

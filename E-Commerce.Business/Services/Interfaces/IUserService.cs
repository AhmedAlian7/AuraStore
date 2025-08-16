
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Customer;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<PaginatedList<CustomerViewModel>> GetAllAsync(int page);

        Task<bool> DeleteUserAsync(string id);




    }
}

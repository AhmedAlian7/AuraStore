
using E_Commerce.Business.ViewModels.Customer;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<CustomerViewModel>> GetAllAsync(int page);

        Task<bool> DeleteUserAsync(string id);




    }
}

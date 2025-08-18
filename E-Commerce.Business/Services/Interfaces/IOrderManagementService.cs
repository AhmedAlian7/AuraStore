
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Admin;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IOrderManagementService
    {

        Task<PaginatedList<OrderViewModel>> GetAllOrdersAsync(int page);


    }
}

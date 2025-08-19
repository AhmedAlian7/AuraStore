
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Order;

namespace E_Commerce.Business.Services.Interfaces
{
    public interface IOrderManagementService
    {

        Task<PaginatedList<OrderViewModel>> GetAllOrdersAsync(int page);
        Task<PaginatedList<OrderViewModel>> GetAllOrdersByUserIdAsync(string id ,int page);
        Task<bool> DeleteOrderAsync(string orderid, string userid);
        Task<OrderDetailsViewModel> GetOrderDetailsAsync(string orderid);

    }

}

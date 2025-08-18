using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Admin;
using E_Commerce.Business.ViewModels.Customer;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Repositories.Interfaces;

namespace E_Commerce.Business.Services.Implementation
{
    public class OrderManagementService : IOrderManagementService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderService _OrderService;

        public OrderManagementService(IUnitOfWork unitOfWork, IOrderService orderService)
        {
            _unitOfWork = unitOfWork;
            _OrderService = orderService;
        }

        public async Task<PaginatedList<OrderViewModel>> GetAllOrdersAsync(int page)
        {
            var orders = await _unitOfWork.Orders.GetAllWithItemsAsync();
            var models = orders.Select(o => new OrderViewModel
            {

                Id = o.Id,
                TotalAmount = o.TotalAmount,
                TotalItems = o.TotalItems,
                UserId = o.UserId,
                OrderStatus = o.OrderStatus,
                
            });

            return PaginatedList<OrderViewModel>.Create(models, page, Numbers.DefaultPageSize);

        }

        public async Task<bool> DeleteOrderAsync(string orderid,string userid)
        {
            var order = await _OrderService.GetOrderAsync(orderid, userid);
           
            if (order == null)
            {
                return false;
            }

            order.IsDeleted = true;
            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}

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

        public OrderManagementService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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

        

    }
}

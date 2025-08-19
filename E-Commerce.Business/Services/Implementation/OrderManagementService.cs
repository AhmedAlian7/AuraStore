using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels;
using E_Commerce.Business.ViewModels.Order;
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
            // Use IQueryable for paging
            var ordersQuery =  _unitOfWork.Orders.GetAllWithItemsAsync().Result.AsQueryable().OrderByDescending(o => o.CreatedAt);
            var totalCount = ordersQuery.Count();
            var pagedOrders = ordersQuery
                .Skip((page - 1) * Numbers.DefaultPageSize)
                .Take(Numbers.DefaultPageSize)
                .ToList();

            var models = pagedOrders.Select(o => new OrderViewModel
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                TotalItems = o.TotalItems,
                CreatedAt = o.CreatedAt,
                OrderStatus = o.OrderStatus,
                UserId = o.UserId,
               
            });

            return new PaginatedList<OrderViewModel>(models, totalCount, page, Numbers.DefaultPageSize);
        }

        public async Task<OrderDetailsViewModel> GetOrderDetailsAsync(string orderid)
        {
            var order = await _OrderService.GetOrderAsync(orderid);
            var model = new OrderDetailsViewModel
            {

                Id = order.Id,
                userName = order.User.ToString(),
                TotalAmount = order.TotalAmount,
                OrderItems = order.OrderItems,
                TotalItems = order.TotalItems,
                CreatedAt = order.CreatedAt,
                OrderStatus = order.OrderStatus,

            };

            return model;
        }

        public async Task<PaginatedList<OrderViewModel>> GetAllOrdersByUserIdAsync(string id, int page)
        {
            var orders = await _unitOfWork.Orders.GetByUserIdAsync(id);

            var models = orders.Where(o => !o.IsDeleted).Select(o => new OrderViewModel
            {

                Id = o.Id,
                TotalAmount = o.TotalAmount,
                TotalItems = o.TotalItems,
                CreatedAt = o.CreatedAt,
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

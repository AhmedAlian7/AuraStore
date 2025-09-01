using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize(Roles = AppRoles.Customer)]
    public class OrdersController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IOrderService _orderService;
        private readonly UserManager<ApplicationUser> _userManager;
        public OrdersController(IOrderManagementService orderManagementService, UserManager<ApplicationUser> userManager ,IOrderService orderService  )
        {
            _orderManagementService = orderManagementService;
            _userManager = userManager;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
           
            var orders = await _orderManagementService.GetAllOrdersByUserIdAsync(userId, page);
            return View(orders);
        }

        public async Task<IActionResult> ViewDetails(string id)
        {

            var order = await _orderManagementService.GetOrderDetailsAsync(id);

            return View(order);

        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(string id ,OrderStatus orderStatus)  
        {
            if (orderStatus == OrderStatus.Pending)
            {
                await _orderService.UpdateOrderStatusAsync(id, OrderStatus.Cancelled);
            }
                return RedirectToAction("Index");
        }
    }
}

using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public class OrderController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;

        public OrderController (IOrderManagementService orderManagementService)
        {
            _orderManagementService = orderManagementService;
        }


        public async Task<IActionResult> Index(int page = 1)
        {

            var orders = await _orderManagementService.GetAllOrdersAsync(page);

            return View(orders);
        }

        //public async Task<IActionResult> Update(int page = 1)
        //{

        //    var orders = await _orderManagementService.G(page);

        //    return View(orders);
        //}
    }
}

using CloudinaryDotNet.Actions;
using E_Commerce.Business.Services.Implementation;
using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Enums;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Stripe;
using System.Numerics;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace E_Commerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public class OrderController : Controller
    {
        private readonly IOrderManagementService _orderManagementService;
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        public OrderController (IOrderManagementService orderManagementService, IOrderService orderService, IEmailService emailService )
        {
            _orderManagementService = orderManagementService;
            _orderService = orderService;
            _emailService = emailService;
        }


        public async Task<IActionResult> Index(int page = 1)
        {

            var orders = await _orderManagementService.GetAllOrdersAsync(page);

            return View(orders);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(string orderid, OrderStatus status)
        {

            var emailBody = "";

            if (status == OrderStatus.Shipped)
            {
                emailBody = $@"<h2 style='color: #4CAF50;'>Order Update</h2>
                               <p>
                                 Hello,<br /><br />
                                 We wanted to let you know that your order has been updated.
                               </p>
                               
                               <p>
                                 <strong>Order ID:</strong> {orderid}<br />
                                 <strong>Status:</strong> <span style='color:#2196F3;font-weight:bold;'>{status}</span>
                               </p>
                               
                               <p>
                                 The order is currently <b>{status}</b> and is expected to be delivered within <b>7 days</b>.
                               </p>";
            }

            else
            {
                emailBody = $@"<h2 style='color:#4CAF50;'>Order Update</h2>
                               <p>
                                 <strong>Order ID:</strong> {orderid}<br />
                                 <strong>Status:</strong> <span style='color:#2196F3;font-weight:bold;'>{status}</span>
                               </p>
                               
                               <p>
                                 Your order status has been successfully updated. Please check the details below:
                               </p>
                               
                               <ul style='line-height:1.6;'>
                                 <li>Order will be processed promptly.</li>
                                 <li>Delivery is expected within <b>7 days</b>.</li>
                                 <li>You will receive further notifications as the status changes.</li>
                               </ul>
                               
                               <p>
                                 Thank you for choosing us! </p>";
   
            }

            var order = await _orderService.GetOrderAsync(orderid);
            await _emailService.SendEmailAsync(order.User.Email, "Your Order Confirmation", emailBody);

            await _orderService.UpdateOrderStatusAsync(orderid, status);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string orderid,string userid)
        {
           
            var emailbody =$@"
                          <h2 style='color:#dc3545;'>Order Cancelled</h2>
                          <p>
                            <strong>Order ID:</strong> {orderid}<br />
                            <strong>Status:</strong> <span style='color:#dc3545;font-weight:bold;'>Deleted</span>
                          </p>
                          
                          <p>
                            We’re writing to let you know that your order has been <b>deleted from our system</b>.  
                            This means it will no longer be processed or delivered.
                          </p>
                          
                          <ul style='line-height:1.6;'>
                            <li>If this deletion was made by mistake, please contact our support team immediately.</li>
                            <li>Any payments associated with this order (if applicable) will be handled according to our refund policy.</li>
                            <li>You may place a new order anytime through our store.</li>
                          </ul>
                          
                          <p>
                            We’re sorry to see this order cancelled, but we hope to serve you again soon.  
                          </p>
                          ";

            var order = await _orderService.GetOrderAsync(orderid);
            await _emailService.SendEmailAsync(order.User.Email, "Your Order Deletation", emailbody);

            await _orderManagementService.DeleteOrderAsync(orderid, userid);
            return RedirectToAction("Index");
        }

    }
}

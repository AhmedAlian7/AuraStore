using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace E_Commerce.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitIfWork;
       
        public UserController(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitIfWork = unitOfWork;
        }
        public async Task<IActionResult> Index(int page =1)
        {
            var users = await _userService.GetAllAsync(page);

            return View("Index", users);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userService.DeleteUserAsync(id);

            if (!user)
            {
                TempData["ErrorMessage"] = "User not found.";
            }
            else
            {
                TempData["SuccessMessage"] = "User deleted successfully.";
            }

            return RedirectToAction(nameof(Index),nameof(User), new { area = "Admin" });
        }

    }
}

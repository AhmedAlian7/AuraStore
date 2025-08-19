using E_Commerce.Business.Services.Interfaces;
using E_Commerce.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using System.Security.Claims;

namespace E_Commerce.Web.Areas.Customer.Controllers
{

    [Area("Customer")]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public ProfileController(IUserService userService , UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager )
        {
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var model = await _userService.ShowProfile(userId);

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(string oldPassword, string newPassword)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var user = await _userManager.FindByIdAsync(userId); 
            var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Current password is incorrect.";
                return View();  
            }
            TempData["SuccessMessage"] = "Password changed successfully.";

            await _signInManager.SignOutAsync();
            return RedirectToAction("Login", "Account", new { area = "Authentication" });
        }
    }
}

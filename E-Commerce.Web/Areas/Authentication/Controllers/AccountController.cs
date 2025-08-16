using E_Commerce.Business.ViewModels.Authentication;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace E_Commerce.Web.Areas.Authentication.Controllers
{
    [Area("Authentication")]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {

            if (User.IsInRole(AppRoles.Admin))
            {
                var roleList = new List<SelectListItem>
                {
                    new() { Text = "Admin", Value = "Admin" },
                    new() { Text = "Customer", Value = "Customer" }
                };
                var registerViewModel = new RegisterViewModel { RolesList = roleList };
                return View(nameof(Register), registerViewModel);
            }
            return View(nameof(Register));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please make sure all fields are valid.";
                return View(nameof(Register), register);
            }

            bool isAdmin = User.FindFirstValue(ClaimTypes.Role) == AppRoles.Admin;
            var user = new ApplicationUser
            {
                Email = register.Email,
                UserName = await GenerateUniqueUserNameAsync(register.Email.Split('@')[0]),
                CreatedAt = DateTime.Now,
            };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                if (isAdmin)
                {
                    await _userManager.AddToRoleAsync(user, register.Role);
                    TempData["SuccessMessage"] = "Account Created successfully!";
                    return RedirectToAction("Index", "Product", new { area = "Admin" });
                }

                await _userManager.AddToRoleAsync(user, AppRoles.Customer);
                await _signInManager.SignInAsync(user, isPersistent: true);
                TempData["SuccessMessage"] = "Account Created successfully!";
                return RedirectToAction("Index", "Home", new { area = "Customer"});
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);

                TempData["ErrorMessage"] = "Failed to create account.";
                return View(nameof(Register), register);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
                return View(model);
            }

            // lockout check
            if (_userManager.SupportsUserLockout &&
                user.LockoutEnd.HasValue &&
                user.LockoutEnd.Value > DateTimeOffset.UtcNow &&
                !user.IsActive)
            {
                ModelState.AddModelError(string.Empty, "This account is locked. Try again later.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                if (await _userManager.IsInRoleAsync(user, AppRoles.Admin))
                    return RedirectToAction("Index", "Product", new { area = "Admin" });

                TempData["SuccessMessage"] = "Login Successfully, Wellcome Back!";
                return RedirectToAction("Index", "Home", new { area = "Customer" });
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Too many attempts. Your account is temporarily locked.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            //HttpContext.Session.SetInt32("CardNumber", 0);
            TempData["success"] = "Logout Successfully";
            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        public IActionResult ExternalLogin(string provider, string? returnUrl = "")
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { area = "Authentication", returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                TempData["error"] = $"Login failed: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["error"] = "Error while Creating account.";
                return RedirectToAction(nameof(Login));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            if (signInResult.Succeeded)
            {
                TempData["success1"] = $"Login with {info.LoginProvider} successful";

                var userSignedIn = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                return RedirectToDashboardOrHome(userSignedIn!, returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? $"{info.ProviderKey}@{info.LoginProvider}.com";
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0];
            var userName = await GenerateUniqueUserNameAsync(name);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    CreatedAt = DateTime.Now
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    TempData["error"] = "Error while creating user.";
                    return RedirectToAction(nameof(Login));
                }
                await _userManager.AddToRoleAsync(user, AppRoles.Customer);
            }

            var alreadyLinked = (await _userManager.GetLoginsAsync(user))
                .Any(login => login.LoginProvider == info.LoginProvider && login.ProviderKey == info.ProviderKey);

            if (!alreadyLinked)
            {
                var linkResult = await _userManager.AddLoginAsync(user, info);
                if (!linkResult.Succeeded)
                {
                    TempData["error"] = "Error while linking external login.";
                    return RedirectToAction(nameof(Login));
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: true);
            TempData["success1"] = $"Login with {info.LoginProvider} successful";
            return RedirectToDashboardOrHome(user, returnUrl);
        }
        private IActionResult RedirectToDashboardOrHome(ApplicationUser user, string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                return LocalRedirect(returnUrl);

            if (_userManager.IsInRoleAsync(user, AppRoles.Admin).Result)
                return RedirectToAction("Dashboard", "User", new { area = "Admin" });

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        private async Task<string> GenerateUniqueUserNameAsync(string rawName)
        {
            var cleaned = Regex.Replace(rawName, @"[^a-zA-Z0-9]", "");

            if (string.IsNullOrWhiteSpace(cleaned))
                cleaned = "user" + Guid.NewGuid().ToString("N").Substring(0, 6);

            string finalName = cleaned;
            int i = 1;

            while (await _userManager.FindByNameAsync(finalName) != null)
            {
                finalName = $"{cleaned}{i}";
                i++;
            }
            return finalName;
        }

        public async Task<IActionResult> IsEmailInUse(string Email)
        {

            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                return Json($"Email '{Email}' is already in use.");
            }
            return Json(true);


        }
    }
}

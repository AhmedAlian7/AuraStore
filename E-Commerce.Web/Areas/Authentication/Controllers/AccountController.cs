using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Authentication;
using E_Commerce.DataAccess.Constants;
using E_Commerce.DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
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
        private readonly IEmailService _emailService;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
            , IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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
                user.LockoutEnd.Value > DateTimeOffset.UtcNow ||
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


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                TempData["SuccessMessage"] = "If an account with that email exists, a password reset link has been sent.";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Account", new { area = "Authentication", userId = user.Id, token = token }, protocol: HttpContext.Request.Scheme);

            var emailBody = $@"
                <!DOCTYPE html>
                <html lang='en'>
                <head>
                    <meta charset='UTF-8'>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                    <style>
                        .btn {{
                            display: inline-block;
                            padding: 12px 24px;
                            margin: 20px 0;
                            font-size: 16px;
                            color: #fff !important;
                            background-color: #007bff;
                            border-radius: 6px;
                            text-decoration: none;
                        }}
                        .btn:hover {{
                            background-color: #0056b3;
                        }}
                        .container {{
                            font-family: Arial, sans-serif;
                            max-width: 600px;
                            margin: auto;
                            padding: 20px;
                            line-height: 1.6;
                            color: #333;
                        }}
                        .footer {{
                            margin-top: 30px;
                            font-size: 12px;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Password Reset Request</h2>
                        <p>Hello,</p>
                        <p>You are receiving this email because we received a password reset request for your account.</p>
                        <p>
                            Please reset your password by clicking the button below:
                        </p>
                        <p>
                            <a href='{callbackUrl}' class='btn'>Reset Password</a>
                        </p>
                        <p>This link will expire in 24 hours for your security.</p>
                        <p>If you did not request a password reset, no further action is required.</p>
                        <p>Best regards,<br><strong>AuraStore Team</strong></p>
                        <div class='footer'>
                            <p>This is an automated message, please do not reply.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await _emailService.SendEmailAsync(model.Email, "Password Reset", emailBody);


            TempData["SuccessMessage"] = "If an account with that email exists, a password reset link has been sent.";
            return RedirectToAction(nameof(ForgotPassword));
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
            {
                return BadRequest("Invalid password reset link.");
            }

            var model = new ResetPasswordViewModel { UserId = userId, Token = token };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                TempData["SuccessMessage"] = "Password reset successful.";
                return RedirectToAction("Login", "Account", new { area = "Authentication" });
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = "Password has been reset successfully.";
                return RedirectToAction("Login", "Account", new { area = "Authentication" });
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }





        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public IActionResult DataDeletion()
        {
            return View();
        }


        // HELPERS
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

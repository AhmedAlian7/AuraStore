using E_Commerce.Business.Services.Interfaces;
using E_Commerce.Business.ViewModels.Product;
using E_Commerce.Business.ViewModels.PromoCode;
using E_Commerce.DataAccess.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Web.Areas.Admin.Controllers
{

    [Area("Admin")]
    [Authorize(Roles = AppRoles.Admin)]
    public class PromoCodeController : Controller
    {
        private readonly IPromoCodeService _promoCodeService;
        public PromoCodeController (IPromoCodeService promoCodeService)
        {
            _promoCodeService = promoCodeService;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            var models = await _promoCodeService.GetAllAsync(page);

            return View(models);
        }

        [HttpGet]
        public IActionResult Add()
        {
            var addCode = new PromoCodeViewModel();
          
            return View("Add", addCode);

        }

        [HttpPost]
        public async Task<IActionResult> Add(PromoCodeViewModel promoCode)
        {
            if (!ModelState.IsValid)
            {

                TempData["ErrorMessage"] = "Please correct the errors in the form.";
                return View("Add", promoCode);
            }
            await _promoCodeService.AddCodeAsync(promoCode);
            TempData["SuccessMessage"] = "Promo Code added successfully.";
            return RedirectToAction("Index");
        }
    }
}

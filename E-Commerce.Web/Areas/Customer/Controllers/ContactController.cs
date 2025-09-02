using E_Commerce.Business.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Text;
using E_Commerce.Business.ViewModels.Contact;

namespace E_Commerce.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class ContactController : Controller
    {
        private readonly IEmailService _emailService;

        public ContactController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new ContactFormViewModel();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ContactFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var subject = $"Contact Form: {model.InquiryType} - {model.Subject}";
                var emailBody = ComposeEmailBody(model);
                var adminEmail = DataAccess.Constants.Emails.AdminMail;
                await _emailService.SendEmailAsync(
                    adminEmail,
                    subject,
                    emailBody
                );

                TempData["ContactSuccess"] = "Thank you for contacting us! We'll get back to you within 24 hours.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Sorry, we couldn't send your message at this time. Please try again later.");
                return View(model);
            }
        }

        private string ComposeEmailBody(ContactFormViewModel model)
        {
            var orderNumberSection = !string.IsNullOrWhiteSpace(model.OrderNumber)
                ? $"<div class='field'><span class='label'>Order Number:</span><span class='value'>{model.OrderNumber}</span></div>"
                : "";

            return $@"<!DOCTYPE html>
                <html>
                <head>
                <style>
                body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                .header {{ background-color: #f4f4f4; padding: 20px; border-bottom: 2px solid #ddd; }}
                .content {{ padding: 20px; }}
                .field {{ margin-bottom: 15px; }}
                .label {{ font-weight: bold; color: #555; }}
                .value {{ margin-left: 10px; }}
                .message {{ background-color: #f9f9f9; padding: 15px; border-left: 4px solid #007bff; margin-top: 20px; }}
                </style>
                </head>
                <body>
                <div class='header'>
                <h2>New Contact Form Submission</h2>
                </div>
                <div class='content'>
                <div class='field'><span class='label'>From:</span><span class='value'>{model.Name} ({model.Email})</span></div>
                <div class='field'><span class='label'>Inquiry Type:</span><span class='value'>{model.InquiryType}</span></div>
                <div class='field'><span class='label'>Subject:</span><span class='value'>{model.Subject}</span></div>
                {orderNumberSection}
                <div class='field'><span class='label'>Submitted:</span><span class='value'>{DateTime.Now:yyyy-MM-dd HH:mm:ss}</span></div>
                <div class='message'>
                <h3>Message:</h3>
                <p>{model.Message.Replace("\n", "<br>")}</p>
                </div>
                </div>
                </body>
                </html>"
            ;
        

        }

    }
}

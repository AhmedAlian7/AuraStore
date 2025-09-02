using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace E_Commerce.Business.ViewModels.Contact
{
    public class ContactFormViewModel
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [StringLength(254, ErrorMessage = "Email cannot exceed 254 characters")]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select an inquiry type")]
        [Display(Name = "Inquiry Type")]
        public string InquiryType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Subject is required")]
        [StringLength(200, ErrorMessage = "Subject cannot exceed 200 characters")]
        [Display(Name = "Subject")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "Message is required")]
        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Message must be between 10 and 2000 characters")]
        [Display(Name = "Message")]
        public string Message { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Order number cannot exceed 50 characters")]
        [Display(Name = "Order Number (Optional)")]
        [RegularExpression(@"^[A-Za-z0-9\-_]*$", ErrorMessage = "Order number can only contain letters, numbers, hyphens, and underscores")]
        public string? OrderNumber { get; set; }

        // Static property for inquiry type options
        public static List<SelectListItem> InquiryTypeOptions => new()
        {
            new SelectListItem { Value = "", Text = "-- Please select --", Disabled = true },
            new SelectListItem { Value = "General Inquiry", Text = "General Inquiry" },
            new SelectListItem { Value = "Order Support", Text = "Order Support" },
            new SelectListItem { Value = "Returns", Text = "Returns & Exchanges" },
            new SelectListItem { Value = "Technical Issue", Text = "Technical Issue" },
            new SelectListItem { Value = "Partnership", Text = "Partnership Opportunities" },
            new SelectListItem { Value = "Billing", Text = "Billing & Payment" },
            new SelectListItem { Value = "Shipping", Text = "Shipping & Delivery" },
            new SelectListItem { Value = "Product Question", Text = "Product Questions" }
        };
    }
}

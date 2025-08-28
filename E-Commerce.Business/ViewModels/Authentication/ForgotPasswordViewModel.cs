using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Business.ViewModels.Authentication
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }

}
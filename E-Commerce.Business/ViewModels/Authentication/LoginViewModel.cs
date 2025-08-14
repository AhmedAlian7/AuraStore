using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Business.ViewModels.Authentication
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;
        public bool RememberMe { get; set; } = false;

    }
}

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace E_Commerce.Business.ViewModels.Authentication
{
    public class RegisterViewModel
    {

        [Remote(action: "IsEmailInUse", controller: "Account", areaName: "Authentication", ErrorMessage = "Email is already Used ,Try different Email")]
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password Must be Bigger than 6 char")]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password Not Match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;

        public string? Role { get; set; }
        public IEnumerable<SelectListItem>? RolesList { get; set; } = new List<SelectListItem>();
    }
}

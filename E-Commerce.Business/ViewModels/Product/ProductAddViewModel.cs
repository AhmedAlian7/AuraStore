using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Business.ViewModels.Product
{
    public class ProductAddViewModel
    {
        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name must not exceed 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(2000, ErrorMessage = "Description must not exceed 2000 characters")]
        public string? Description { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal? DiscountPrice { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock count cannot be negative.")]
        public int StockCount { get; set; }

       
        [Required(ErrorMessage = "Category is required.")]
        public int CategoryId { get; set; }


        public IEnumerable<SelectListItem> Categories { get; set; } = new List<SelectListItem>();

        [Required(ErrorMessage = "Main Image is required.")]
        public IFormFile MainImageFile { get; set; } = null!;

        public List<IFormFile> AdditionalImages { get; set; } = new();

        public string? MainImageUrl { get; set; } = string.Empty;
    }
}


using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Business.ViewModels.Dtos
{
    public class ProductPostDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockCount { get; set; }
        public int CategoryId { get; set; }

        public IFormFile ImageFile { get; set; } = null!;
        public string? MainImageUrl { get; set; } = string.Empty;
    }
}

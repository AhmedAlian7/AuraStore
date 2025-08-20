using System;

namespace E_Commerce.Business.ViewModels.Dtos
{
	public class ProductDto
	{
		public int Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public decimal? DiscountPrice { get; set; }
		public int StockCount { get; set; }
		public string ImageUrl { get; set; } = string.Empty;
		public int CategoryId { get; set; }
		public string CategoryName { get; set; } = string.Empty;
		public double Rating { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.DataAccess.Entities
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? DiscountPrice { get; set; }
        public int StockCount { get; set; }
        public string MainImageUrl { get; set; } = string.Empty;

        [NotMapped]
        public bool InStock => StockCount > 0;
        [NotMapped]
        public decimal EffectivePrice => DiscountPrice ?? Price;
        [NotMapped]
        public double AverageRating => Reviews?.Any() == true ?
            Reviews.Average(r => r.Rating) : 0;

        [NotMapped]
        public int ReviewCount => Reviews?.Count ?? 0;

        // Navigation properties
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public ICollection<ProductNotification> ProductNotifications { get; set; } = new List<ProductNotification>();

    }
}


using E_Commerce.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.DataAccess.Entities
{
    public class Order : BaseEntity<int>
    {
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; } = 0;
        public decimal ShippingCost { get; set; } = 0;
        public decimal DiscountAmount { get; set; } = 0;

        public string? Description { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public DateTime? ShippedDate { get; set; }

        // Calculated properties
        [NotMapped]
        public decimal TotalAmount => OrderItems?.Sum(oi => oi.Quantity * oi.Product.EffectivePrice) ?? 0;
        [NotMapped]
        public int TotalItems => OrderItems?.Sum(oi => oi.Quantity) ?? 0;

        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}

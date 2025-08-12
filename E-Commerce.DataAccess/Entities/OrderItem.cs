using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.DataAccess.Entities
{
    public class OrderItem : BaseEntity<int>
    {
        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; } // Price at the time of order

        public decimal DiscountAmount { get; set; } = 0;

        [NotMapped]
        public decimal Subtotal => (UnitPrice * Quantity) - DiscountAmount;
    }
}

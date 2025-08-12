using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.DataAccess.Entities
{
    public class Cart : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // derived properties
        [NotMapped]
        public decimal TotalAmount => CartItems?.Sum(ci => ci.Quantity * ci.Product.EffectivePrice) ?? 0;

        [NotMapped]
        public int TotalItems => CartItems?.Sum(ci => ci.Quantity) ?? 0;
    }
}

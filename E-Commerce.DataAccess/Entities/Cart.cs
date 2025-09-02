using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.DataAccess.Entities
{
    public class Cart : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

        // Promo code related properties
        public int? PromoCodeId { get; set; }
        public PromoCode? PromoCode { get; set; }
        public decimal? DiscountAmount { get; set; }

        // derived properties
        [NotMapped]
        public decimal TotalAmount => CartItems?.Where(ci => ci.Product != null).Sum(ci => ci.Quantity * ci.Product.EffectivePrice) ?? 0;

        [NotMapped]
        public decimal DiscountedTotal => Math.Max(TotalAmount - (DiscountAmount ?? 0), 0);

        [NotMapped]
        public int TotalItems => CartItems?.Sum(ci => ci.Quantity) ?? 0;
    }
}

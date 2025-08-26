using E_Commerce.DataAccess.Enums;

namespace E_Commerce.DataAccess.Entities
{
    public class PromoCode : BaseEntity<int>
    {
        public string Code { get; set; } = null!;
        public string? Description { get; set; }
        public DiscountType DiscountType { get; set; }
        public decimal DiscountValue { get; set; } // Percentage or fixed amount based on DiscountType
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public bool IsActive { get; set; }

        // Relationships
        public ICollection<Cart> Carts { get; set; } = new List<Cart>();
    }


}

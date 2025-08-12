namespace E_Commerce.DataAccess.Entities
{
    public class Review : BaseEntity<int>
    {
        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int Rating { get; set; }

        public string? Comment { get; set; }

        public bool IsVerifiedPurchase { get; set; } = false;
    }
}

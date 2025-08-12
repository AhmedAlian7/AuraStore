namespace E_Commerce.DataAccess.Entities
{
    public class ProductImage : BaseEntity<int>
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public string ImageUrl { get; set; } = string.Empty;
        public int DisplayOrder { get; set; } = 0;
        public bool IsMain { get; set; } = false;
    }
}

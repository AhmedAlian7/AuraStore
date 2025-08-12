    
namespace E_Commerce.DataAccess.Entities
{
    public class CartItem : BaseEntity<int>
    {
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public int Quantity { get; set; }
    }
}

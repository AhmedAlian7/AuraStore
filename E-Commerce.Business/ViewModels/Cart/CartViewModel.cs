namespace E_Commerce.Business.ViewModels.Cart
{
    public class CartViewModel
    {
        public int CartId { get; set; }
        public List<CartItemViewModel> Items { get; set; } = new();
        public decimal TotalAmount { get; set; }
        public decimal SubTotal { get; set; }
        public int TotalItems { get; set; }
        public decimal TaxAmount { get; set; }
    }
}

namespace E_Commerce.Business.ViewModels.Cart
{
    public class CartSummaryDto
    {
        public int TotalItems { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}

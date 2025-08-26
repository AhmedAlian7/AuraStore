
namespace E_Commerce.Business.ViewModels.Cart
{
    public class PromoCodeResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public CartSummaryDto? CartSummary { get; set; }
    }
}

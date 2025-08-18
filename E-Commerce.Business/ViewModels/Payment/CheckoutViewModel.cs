namespace E_Commerce.Business.ViewModels.Payment
{
    public class CheckoutViewModel
    {
        public string? SessionId { get; set; }
        public string? OrderId { get; set; }
        public List<CheckoutItemViewModel> Items { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public string Currency { get; set; } = "USD";

        public string ShippingAddressId { get; set; } = string.Empty;
        public string? DiscountCode { get; set; }
    }

    public class CheckoutItemViewModel
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Total => UnitPrice * Quantity;
        public string? ImageUrl { get; set; }
    }
}

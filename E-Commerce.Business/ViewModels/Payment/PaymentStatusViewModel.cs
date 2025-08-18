namespace E_Commerce.Business.ViewModels.Payment
{
    public class PaymentStatusViewModel
    {
        public string OrderId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? PaymentIntentId { get; set; }
        public string? SessionId { get; set; }
        public DateTime? PaidAt { get; set; }
        public bool IsSuccess => Status == "paid";
        public List<OrderItemViewModel> Items { get; set; } = new();
        public string CustomerEmail { get; set; } = string.Empty;
    }
    public class OrderItemViewModel
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total { get; set; }
    }
}

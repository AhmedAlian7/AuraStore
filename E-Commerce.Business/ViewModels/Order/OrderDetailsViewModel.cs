
using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;

namespace E_Commerce.Business.ViewModels.Order
{
    public class OrderDetailsViewModel
    {
        public int Id { get; set; }
        public string? userName { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int TotalItems { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}

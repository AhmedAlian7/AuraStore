using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.Business.ViewModels.Admin
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? CreatedAt  { get; set; }
        public int TotalItems { get; set; }
        public string UserId { get; set; }
        public OrderStatus OrderStatus { get; set; }



    }
}

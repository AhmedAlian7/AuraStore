
namespace E_Commerce.Business.ViewModels.Customer
{
    public class CustomerViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreateAt { get; set; }
        public int OrdersCount { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Commerce.DataAccess.Entities
{
    public class ProductNotification : BaseEntity<int>
    {
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public bool IsNotified { get; set; } = false;
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public Product Product { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}

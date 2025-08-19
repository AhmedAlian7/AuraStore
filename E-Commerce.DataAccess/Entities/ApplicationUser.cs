
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.DataAccess.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public int CartId { get; set; }
        public Cart Cart { get; set; } = null!;
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Review> Reviews { get; set; } = new List<Review>();

    }
}

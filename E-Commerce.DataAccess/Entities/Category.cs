
namespace E_Commerce.DataAccess.Entities
{
    public class Category : BaseEntity<int>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        // Navigation properties
        public ICollection<Product> Products { get; set; } = new List<Product>();

    }
}

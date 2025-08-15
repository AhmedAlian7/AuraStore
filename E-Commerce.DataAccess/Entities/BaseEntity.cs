
using System.ComponentModel.DataAnnotations;

namespace E_Commerce.DataAccess.Entities
{
    public abstract class BaseEntity<T>
    {
        public T Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }


}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Data.Config
{
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);


            builder.HasKey(c => c.Id);

            builder.Property(c => c.UserId)
                .IsRequired();

            // Ignore computed properties
            builder.Ignore(c => c.TotalAmount);
            builder.Ignore(c => c.TotalItems);

            // Relationships
            builder.HasOne(c => c.User)
                .WithOne(u => u.Cart)
                .HasForeignKey<Cart>(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.CartItems)
                .WithOne(ci => ci.Cart)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}

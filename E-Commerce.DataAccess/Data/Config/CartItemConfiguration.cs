using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Data.Config
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);


            builder.HasKey(ci => ci.Id);

            builder.Property(ci => ci.Quantity)
                .IsRequired();

            // Relationships
            builder.HasOne(ci => ci.Cart)
                .WithMany(c => c.CartItems)
                .HasForeignKey(ci => ci.CartId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ci => ci.Product)
                .WithMany(p => p.CartItems)
                .HasForeignKey(ci => ci.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}

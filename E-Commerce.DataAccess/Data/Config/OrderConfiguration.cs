using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Data.Config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);


            builder.HasKey(o => o.Id);

            builder.Property(o => o.SubTotal)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(o => o.TaxAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(o => o.ShippingCost)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(o => o.DiscountAmount)
                .HasColumnType("decimal(18,2)")
                .HasDefaultValue(0);

            builder.Property(o => o.Description)
                .HasMaxLength(1000)
                .IsUnicode(true);

            builder.Property(o => o.OrderStatus)
                .IsRequired();

            builder.Property(o => o.UserId)
                .IsRequired();

            // Ignore computed properties
            builder.Ignore(o => o.TotalAmount);
            builder.Ignore(o => o.TotalItems);

            // Relationships
            builder.HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }

}

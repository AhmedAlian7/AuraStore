using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Data.Config
{

    public partial class CategoryConfiguration
    {
        public class ProductConfiguration : IEntityTypeConfiguration<Product>
        {
            public void Configure(EntityTypeBuilder<Product> builder)
            {
                builder.HasQueryFilter(a => !a.IsDeleted);

                builder.HasKey(p => p.Id);

                builder.Property(p => p.Name)
                    .IsRequired()
                    .HasMaxLength(50);

                builder.Property(p => p.Description)
                    .HasMaxLength(2000);

                builder.Property(p => p.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                builder.Property(p => p.DiscountPrice)
                    .HasColumnType("decimal(18,2)");

                builder.Property(p => p.StockCount)
                    .IsRequired()
                    .HasDefaultValue(0);

                builder.Property(p => p.MainImageUrl)
                    .HasMaxLength(500);

                // Ignore computed properties
                builder.Ignore(p => p.InStock);
                builder.Ignore(p => p.EffectivePrice);
                builder.Ignore(p => p.AverageRating);
                builder.Ignore(p => p.ReviewCount);

                // Relationships
                builder.HasOne(p => p.Category)
                    .WithMany(c => c.Products)
                    .HasForeignKey(p => p.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(p => p.CartItems)
                    .WithOne(ci => ci.Product)
                    .HasForeignKey(ci => ci.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(p => p.OrderItems)
                    .WithOne(oi => oi.Product)
                    .HasForeignKey(oi => oi.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                builder.HasMany(p => p.Reviews)
                    .WithOne(r => r.Product)
                    .HasForeignKey(r => r.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasMany(p => p.ProductImages)
                    .WithOne(pi => pi.Product)
                    .HasForeignKey(pi => pi.ProductId)
                    .OnDelete(DeleteBehavior.Cascade);

            }
        }
    }
}

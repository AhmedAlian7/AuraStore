using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Data.Config
{
    public class ProductNotificationConfiguration : IEntityTypeConfiguration<ProductNotification>
    {
        public void Configure(EntityTypeBuilder<ProductNotification> builder)
        {
            builder.HasQueryFilter(pn => !pn.IsDeleted);

            builder.HasKey(pn => pn.Id);

            builder.Property(pn => pn.ProductId)
                .IsRequired();

            builder.Property(pn => pn.UserId)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(pn => pn.IsNotified)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(pn => pn.RequestDate)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Relationships
            builder.HasOne(pn => pn.Product)
                .WithMany()
                .HasForeignKey(pn => pn.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pn => pn.User)
                .WithMany()
                .HasForeignKey(pn => pn.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Composite unique constraint to prevent duplicate notifications
            builder.HasIndex(pn => new { pn.ProductId, pn.UserId })
                .IsUnique();
        }
    }
}

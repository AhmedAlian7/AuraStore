using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Data.Config
{
    public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
    {
        public void Configure(EntityTypeBuilder<PromoCode> builder)
        {
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Description)
                .HasMaxLength(255);

            builder.Property(p => p.DiscountValue)
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.MinOrderAmount)
                .HasColumnType("decimal(10,2)");

            builder.HasMany(p => p.Carts)
                .WithOne(o => o.PromoCode)
                .HasForeignKey(o => o.PromoCodeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasIndex(p => p.Code)
                .IsUnique();
        }
    }



}

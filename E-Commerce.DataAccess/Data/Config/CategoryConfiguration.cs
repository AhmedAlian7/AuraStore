using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using E_Commerce.DataAccess.Entities;

namespace E_Commerce.DataAccess.Data.Config
{
    public partial class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);


            builder.HasKey(c => c.Id);


            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            // Relationships
            builder.HasMany(c => c.Products)
                .WithOne(p => p.Category)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);


        }
    }
}

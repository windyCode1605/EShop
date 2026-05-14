
// Services/Core/CR.Core.Infrastructure/Persistence/Configurations/ProductConfiguration.cs
// Tách config ra file riêng — không viết lẫn vào DbContext
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CR.Core.Domain.Catalog;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.HasKey(p => p.Id);
        b.Property(p => p.Name).IsRequired().HasMaxLength(256);
        b.Property(p => p.Slug).IsRequired().HasMaxLength(256).IsUnicode(false);
        b.Property(p => p.BasePrice).HasColumnType("decimal(18,2)");

        b.HasIndex(p => p.CategoryId);
        b.HasQueryFilter(p => !p.Deleted);

        b.HasOne(p => p.Category)
         .WithMany(c => c.Products)
         .HasForeignKey(p => p.CategoryId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasMany(p => p.Images)
        .WithOne( i => i.Product)
        .HasForeignKey(i => i.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(p => p.Variants)
        .WithOne(v => v.Product)
        .HasForeignKey(v => v.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(p => p.Reviews)
        .WithOne(r => r.Product)
        .HasForeignKey(r => r.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

    }
}

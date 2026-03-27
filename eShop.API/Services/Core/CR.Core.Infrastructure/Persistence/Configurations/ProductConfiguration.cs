
// Services/Core/CR.Core.Infrastructure/Persistence/Configurations/ProductConfiguration.cs
// Tách config ra file riêng — không viết lẫn vào DbContext
using CR.Core.Domain.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductConfiguration : IEntityTypeConfiguration<Products>
{
    public void Configure(EntityTypeBuilder<Products> b)
    {
        b.ToTable("Products");
        b.HasKey(p => p.Id);
        b.Property(p => p.Name).IsRequired().HasMaxLength(300);
        b.Property(p => p.Price).HasColumnType("decimal(18,2)");
        b.Property(p => p.AI_Description).HasColumnType("text");

        // Index để tăng tốc tìm kiếm theo CategoryId
        b.HasIndex(p => p.CategoryId);

        // Query filter: tự động loại soft-deleted records khỏi mọi query
        b.HasQueryFilter(p => !p.IsDeleted);

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
using CR.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class ProductAttributeConfiguration : IEntityTypeConfiguration<ProductAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductAttribute> entity)
        {
            entity.HasKey(pa => pa.Id);

            entity.Property(pa => pa.CustomValue).HasMaxLength(255);

            entity.HasIndex(pa => new { pa.ProductId, pa.AttributeId }).IsUnique();

            entity.ToTable(t => t.HasCheckConstraint("CK_PA_ValueXor", "(\"AttributeValueId\" IS NOT NULL AND \"CustomValue\" IS NULL) OR (\"AttributeValueId\" IS NULL AND \"CustomValue\" IS NOT NULL)"));

            entity.HasQueryFilter(pa => !pa.Deleted);

            entity.HasOne(pa => pa.Product)
                .WithMany(p => p.ProductAttributes)
                .HasForeignKey(pa => pa.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pa => pa.Attribute)
                .WithMany()
                .HasForeignKey(pa => pa.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pa => pa.AttributeValue)
                .WithMany()
                .HasForeignKey(pa => pa.AttributeValueId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}

using CR.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class ProductVariantAttributeConfiguration : IEntityTypeConfiguration<ProductVariantAttribute>
    {
        public void Configure(EntityTypeBuilder<ProductVariantAttribute> entity)
        {
            entity.HasKey(pva => pva.Id);

            entity.Property(pva => pva.CustomValue).HasMaxLength(255);

            entity.HasIndex(pva => new { pva.ProductVariantId, pva.AttributeId }).IsUnique();

            entity.ToTable(t => t.HasCheckConstraint("CK_PVA_ValueXor", "(\"AttributeValueId\" IS NOT NULL AND \"CustomValue\" IS NULL) OR (\"AttributeValueId\" IS NULL AND \"CustomValue\" IS NOT NULL)"));

            entity.HasQueryFilter(pva => !pva.Deleted);

            entity.HasOne(pva => pva.ProductVariant)
                .WithMany(pv => pv.VariantAttributes)
                .HasForeignKey(pva => pva.ProductVariantId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(pva => pva.Attribute)
                .WithMany(a => a.ProductVariantAttributes)
                .HasForeignKey(pva => pva.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(pva => pva.AttributeValue)
                .WithMany(av => av.ProductVariantAttributes)
                .HasForeignKey(pva => pva.AttributeValueId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);
        }
    }
}

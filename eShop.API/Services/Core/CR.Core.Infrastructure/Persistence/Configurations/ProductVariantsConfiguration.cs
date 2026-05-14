using CR.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class ProductVariantsConfiguration : IEntityTypeConfiguration<ProductVariant>
    {
        public void Configure(EntityTypeBuilder<ProductVariant> entity)
        {
            entity.Property(v => v.SKU).IsRequired().HasMaxLength(100).IsUnicode(false);
            entity.Property(v => v.PriceAdjustment).HasColumnType("decimal(18,2)");
            entity.Property(v => v.RowVersion).IsRowVersion();
            entity.HasQueryFilter(v => !v.Deleted && !v.Product.Deleted);
        }
    }
}

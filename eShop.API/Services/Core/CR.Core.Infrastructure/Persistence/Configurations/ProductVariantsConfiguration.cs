using CR.Core.Domain.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class ProductVariantsConfiguration : IEntityTypeConfiguration<ProductVariants>
    {
        public void Configure(EntityTypeBuilder<ProductVariants> entity)
        {
            entity.HasQueryFilter(v => !v.Product.IsDeleted);
        }
    }
}

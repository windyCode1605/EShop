    using CR.Core.Domain.Catalog;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CR.Core.Infrastructure.Persistence.Configurations
{

    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> entity)
        {
            entity.HasKey(pi => pi.Id);
            entity.Property(pi => pi.Url).IsRequired().HasMaxLength(500);
            entity.Property(pi => pi.IsPrimary).IsRequired();
            entity.HasQueryFilter(x => !x.Deleted && !x.Product.Deleted);

            entity.HasOne(pi => pi.Product)
                   .WithMany(p => p.Images)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

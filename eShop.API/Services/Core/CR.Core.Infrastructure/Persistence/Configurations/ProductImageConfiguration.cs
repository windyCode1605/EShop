    using CR.Core.Domain.Product;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace CR.Core.Infrastructure.Persistence.Configurations
{

    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImages>
    {
        public void Configure(EntityTypeBuilder<ProductImages> entity)
        {
            entity.ToTable("ProductImages");
            entity.HasKey(pi => pi.Id);
            entity.Property(pi => pi.Url).IsRequired().HasMaxLength(500);
            entity.Property(pi => pi.IsPrimary).IsRequired();
            entity.HasQueryFilter(x => !x.Product.IsDeleted);

            entity.HasOne(pi => pi.Product)
                   .WithMany(p => p.Images)
                   .HasForeignKey(pi => pi.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
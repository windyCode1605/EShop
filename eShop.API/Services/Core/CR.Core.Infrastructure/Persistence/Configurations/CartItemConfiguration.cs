using CR.Core.Domain.Carts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> entity)
        {
            entity.HasQueryFilter(ci => !ci.Deleted);

            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.CartId).IsRequired();
            entity.Property(ci => ci.ProductVariantId).IsRequired();
            entity.Property(ci => ci.Quantity).IsRequired();

            entity.HasOne(ci => ci.Cart)
            .WithMany(c => c.Items)
            .HasForeignKey(ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ci => ci.ProductVariant)
            .WithMany()
            .HasForeignKey(ci => ci.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);
        }

    }
}

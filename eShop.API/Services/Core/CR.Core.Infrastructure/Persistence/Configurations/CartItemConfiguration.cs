using CR.Core.Domain.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class CartItemConfiguration : IEntityTypeConfiguration<CartItems>
    {
        public void Configure(EntityTypeBuilder<CartItems> entity)
        {
            entity.HasQueryFilter(ci => !ci.Variant.Product.IsDeleted);

            entity.HasKey(ci => ci.Id);
            entity.Property(ci => ci.CartId).IsRequired();
            entity.Property(ci => ci.VariantId).IsRequired();
            entity.Property(ci => ci.Quantity).IsRequired();

            entity.HasOne(ci => ci.Cart)
            .WithMany( c => c.Items)
            .HasForeignKey( ci => ci.CartId)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ci => ci.Variant)
            .WithMany( c => c.CartItems)
            .HasForeignKey(ci => ci.VariantId)
            .OnDelete(DeleteBehavior.Cascade);
        }
           
    }
}
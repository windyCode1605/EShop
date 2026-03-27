using CR.Core.Domain.Cart;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class CartConfiguration : IEntityTypeConfiguration<Carts>
    {
        public void Configure(EntityTypeBuilder<Carts> entity)
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.UserId).IsRequired();
            entity.Property(c => c.CreatedAt).IsRequired();

            entity.HasOne(c => c.User)
            .WithOne(u => u.Cart)
            .HasForeignKey<Carts>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(c => c.Items)
            .WithOne(i => i.Cart)
            .HasForeignKey(i => i.CartId)
            .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
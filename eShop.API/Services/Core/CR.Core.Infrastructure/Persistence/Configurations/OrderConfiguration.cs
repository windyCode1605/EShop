using CR.Core.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {

        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.OrderCode).IsRequired().HasMaxLength(50).IsUnicode(false);
            entity.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(o => o.Status).IsRequired().HasMaxLength(50).IsUnicode(false);
            entity.Property(o => o.PaymentMethod).IsRequired().HasMaxLength(50).IsUnicode(false);
            entity.Property(o => o.RowVersion).IsConcurrencyToken();
            entity.HasQueryFilter(o => !o.Deleted);

            entity.HasMany(o => o.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

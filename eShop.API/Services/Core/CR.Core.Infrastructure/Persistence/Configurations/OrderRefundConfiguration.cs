using CR.Core.Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class OrderRefundConfiguration : IEntityTypeConfiguration<OrderRefund>
    {
        public void Configure(EntityTypeBuilder<OrderRefund> entity)
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Status).IsRequired().HasMaxLength(50).IsUnicode(false);
            entity.Property(r => r.Reason).HasMaxLength(500);
            entity.HasQueryFilter(r => !r.Deleted);
        }
    }
}

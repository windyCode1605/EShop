using CR.Core.Domain.Logistics;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class ShipmentConfiguration : IEntityTypeConfiguration<Shipment>
    {
        public void Configure(EntityTypeBuilder<Shipment> entity)
        {
            entity.HasKey(s => s.Id);

            entity.Property(s => s.OrderId).IsRequired();
            entity.Property(s => s.ShippingProvider).IsRequired().HasMaxLength(100);
            entity.Property(s => s.TrackingNumber).HasMaxLength(100).IsUnicode(false);
            entity.Property(s => s.ShippingFee).HasColumnType("decimal(18,2)").IsRequired();
            entity.Property(s => s.ReceiverName).IsRequired().HasMaxLength(256);
            entity.Property(s => s.ReceiverPhone).IsRequired().HasMaxLength(20);
            entity.Property(s => s.ShippingAddress).IsRequired().HasMaxLength(1024);
            entity.Property(s => s.Status).IsRequired().HasMaxLength(50).IsUnicode(false);

            // CẬP NHẬT: Thay đổi HasName thành HasDatabaseName để hết warning
            entity.HasIndex(s => s.TrackingNumber)
                .HasDatabaseName($"IX_{nameof(Shipment)}_TrackingNumber");

            // Query Filter: không hiển thị Shipment bị xóa
            entity.HasQueryFilter(s => !s.Deleted);

            // FK → Order (Cascade: khi hủy đơn thì xóa luôn vận đơn)
            entity.HasOne(s => s.Order)
                .WithMany(o => o.Shipments)
                .HasForeignKey(s => s.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

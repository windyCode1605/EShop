using CR.Core.Domain.Address;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class AddressConfiguration : IEntityTypeConfiguration<Addresses>
    {
        public void Configure(EntityTypeBuilder<Addresses> entity)
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Street).IsRequired().HasMaxLength(100);
            entity.Property(a => a.City).IsRequired().HasMaxLength(50);
            entity.Property(a => a.Province).IsRequired().HasMaxLength(50);
            entity.Property(a => a.IsDefault).IsRequired();

            entity.HasOne(a => a.User)
            .WithMany(u => u.Addresses)
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
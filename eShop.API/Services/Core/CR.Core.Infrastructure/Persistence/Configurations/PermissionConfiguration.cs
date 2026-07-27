using CR.Core.Domain.User;
using CR.Constants.Common.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable(nameof(Permission), DbSchemas.Default);

            builder.HasKey(p => p.PermissionKey);

            builder.Property(p => p.PermissionKey)
                .HasColumnType("varchar(255)")
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.DisplayName)
                .HasMaxLength(255)
                .IsRequired();

            builder.Property(p => p.PermissionGroup)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(500);

            builder.Property(p => p.CreatedDate)
                .IsRequired()
                .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

            // Relationship Permission (1) - RolePermission (N)
            builder.HasMany(p => p.RolePermissions)
                .WithOne(rp => rp.Permission)
                .HasForeignKey(rp => rp.PermissionKey)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

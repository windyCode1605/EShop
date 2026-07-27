using CR.Core.Domain.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<Users>
    {
        public void Configure(EntityTypeBuilder<Users> entity)
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Username).IsRequired().HasMaxLength(50);
            entity.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);

            // Fix: Khai báo RowVersion là optimistic concurrency token
            // SQL Server: tự động quản lý bởi database (timestamp type)
            // PostgreSQL (Npgsql): dùng xmin system column làm concurrency token
            entity.Property(u => u.RowVersion).IsRowVersion();

            entity.HasOne(u => u.Profile)
                  .WithOne()
                  .HasForeignKey<UserProfile>(p => p.UserId);

        }
    }
}
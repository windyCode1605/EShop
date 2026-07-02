using CR.Core.Domain.Opts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class AuthOtpConfiguration : IEntityTypeConfiguration<AuthOtp>
    {
        public void Configure(EntityTypeBuilder<AuthOtp> entity)
        {
            entity.HasKey(a => a.Id);
            entity.Property(a => a.OtpCode).IsRequired().HasMaxLength(128).IsUnicode(false);
            entity.Property(a => a.ExpireTime).IsRequired();
            entity.Property(a => a.IsUsed).IsRequired();
            entity.Property(a => a.VerifyTime).IsRequired();
            entity.Property(a => a.CreatedDate).IsRequired();

            entity.HasOne(a => a.User)
                .WithMany(u => u.AuthOtps)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
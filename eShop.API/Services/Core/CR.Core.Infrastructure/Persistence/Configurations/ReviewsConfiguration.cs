using CR.Core.Domain.Review;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class ReviewsConfiguration : IEntityTypeConfiguration<Reviews>
    {
        public void Configure(EntityTypeBuilder<Reviews> entity)
        {
            entity.HasQueryFilter(r => !r.Product.IsDeleted);

            entity.HasKey(r => r.Id);
            entity.Property(r => r.Comment).IsRequired().HasMaxLength(1000);
            entity.HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
                
        }
    }
}
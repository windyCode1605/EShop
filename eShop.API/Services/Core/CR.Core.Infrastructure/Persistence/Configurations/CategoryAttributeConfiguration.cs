using CR.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class CategoryAttributeConfiguration : IEntityTypeConfiguration<CategoryAttribute>
    {
        public void Configure(EntityTypeBuilder<CategoryAttribute> entity)
        {
            entity.HasKey(ca => ca.Id);

            entity.HasIndex(ca => new { ca.CategoryId, ca.AttributeId }).IsUnique();

            entity.HasQueryFilter(ca => !ca.Deleted);

            entity.HasOne(ca => ca.Category)
                .WithMany(c => c.CategoryAttributes)
                .HasForeignKey(ca => ca.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ca => ca.Attribute)
                .WithMany()
                .HasForeignKey(ca => ca.AttributeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

using CR.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CatalogAttribute = CR.Core.Domain.Catalog.Attribute;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class AttributeConfiguration : IEntityTypeConfiguration<CatalogAttribute>
    {
        public void Configure(EntityTypeBuilder<CatalogAttribute> entity)
        {
            entity.ToTable("Attribute", schema: "dbo", t => t.HasCheckConstraint("CK_Attribute_Type", "AttributeType IN ('Text', 'Number', 'Color', 'Boolean')"));
            entity.HasKey(a => a.Id);

            entity.Property(a => a.Name).IsRequired().HasMaxLength(100);
            entity.Property(a => a.Description).HasMaxLength(500);
            entity.Property(a => a.AttributeType).IsRequired().HasMaxLength(20);

            entity.HasQueryFilter(a => !a.Deleted);

            entity.HasMany(a => a.Values)
                .WithOne(av => av.Attribute)
                .HasForeignKey(av => av.AttributeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

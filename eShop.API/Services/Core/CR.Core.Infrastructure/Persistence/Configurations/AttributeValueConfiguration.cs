using CR.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CR.Core.Infrastructure.Persistence.Configurations
{
    public class AttributeValueConfiguration : IEntityTypeConfiguration<AttributeValue>
    {
        public void Configure(EntityTypeBuilder<AttributeValue> entity)
        {
            entity.HasKey(av => av.Id);

            entity.Property(av => av.Value).IsRequired().HasMaxLength(100);
            entity.Property(av => av.CreatedDate).IsRequired().HasColumnType("datetime");

            entity.HasQueryFilter(av => !av.Deleted);

            // FK -> Attribute đã được cấu hình từ phía AttributeConfiguration
        }
    }
}

using CR.Core.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace CR.Core.ApplicationServices.ProductModule.Implements;

public static class ProductQueryExtensions
{
    public static IQueryable<Product> IncludeFullProductDetails(this IQueryable<Product> query)
    {
        return query
            .Include(p => p.Category)
            .Include(p => p.Images)
            .Include(p => p.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.VariantAttributes.Where(va => !va.Deleted))
                    .ThenInclude(va => va.Attribute)
            .Include(p => p.Variants.Where(v => !v.Deleted))
                .ThenInclude(v => v.VariantAttributes.Where(va => !va.Deleted))
                    .ThenInclude(va => va.AttributeValue);
    }
}

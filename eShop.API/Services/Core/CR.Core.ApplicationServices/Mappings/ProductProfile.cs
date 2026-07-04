using AutoMapper;
using CR.Core.Domain.Catalog;
using CR.Core.Dtos.Product;
using CatalogAttribute = CR.Core.Domain.Catalog.Attribute;

namespace CR.Core.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        // Request -> Entity
        CreateMap<ProductRequestDto, Product>()
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.Price));

        // ProductVariantAttribute -> VariantAttributeDto
        CreateMap<ProductVariantAttribute, VariantAttributeDto>()
            .ForMember(dest => dest.AttributeId,    opt => opt.MapFrom(src => src.AttributeId))
            .ForMember(dest => dest.AttributeName,  opt => opt.MapFrom(src => src.Attribute.Name))
            .ForMember(dest => dest.AttributeType,  opt => opt.MapFrom(src => src.Attribute.AttributeType))
            .ForMember(dest => dest.AttributeValueId, opt => opt.MapFrom(src => src.AttributeValueId))
            .ForMember(dest => dest.AttributeValue, opt => opt.MapFrom(src => src.AttributeValue != null ? src.AttributeValue.Value : null))
            .ForMember(dest => dest.CustomValue,    opt => opt.MapFrom(src => src.CustomValue));

        // ProductVariant -> ProductVariantResponseDto
        CreateMap<ProductVariant, ProductVariantResponseDto>()
            .ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.VariantAttributes));

        // Entity -> Response
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Price,        opt => opt.MapFrom(src => src.BasePrice))
            .ForMember(dest => dest.Stock,        opt => opt.MapFrom(src => src.Variants.Sum(v => v.StockQuantity)))
            .ForMember(dest => dest.AI_Description, opt => opt.Ignore())
            .ForMember(dest => dest.AI_Generated, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.Variants,     opt => opt.MapFrom(src => src.Variants));
    }
}

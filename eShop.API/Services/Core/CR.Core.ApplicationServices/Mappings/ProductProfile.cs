using AutoMapper;
using CR.Core.Domain.Catalog;
using CR.Core.Dtos.Product;

namespace CR.Core.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        // Request -> Entity
        CreateMap<ProductRequestDto, Product>()
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.BasePrice, opt => opt.MapFrom(src => src.Price));

        // Entity -> Response
        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BasePrice))
            .ForMember(dest => dest.Stock, opt => opt.MapFrom(src => src.Variants.Sum(v => v.StockQuantity)))
            .ForMember(dest => dest.AI_Description, opt => opt.Ignore())
            .ForMember(dest => dest.AI_Generated, opt => opt.Ignore())
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name));
    }
}

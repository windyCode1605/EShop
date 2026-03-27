using AutoMapper;
using CR.Core.Domain.Product;
using CR.Core.Dtos.Product;

namespace CR.Core.Application.Mappings;

public class ProductProfile : Profile
{
    public ProductProfile()
    {
        // Request -> Entity
        CreateMap<ProductRequestDto, Products>()
            .ForMember(dest => dest.Slug, opt => opt.Ignore())
            .ForMember(dest => dest.AI_Description, opt => opt.Ignore())
            .ForMember(dest => dest.AI_Generated, opt => opt.Ignore());

        // Entity -> Response
        CreateMap<Products, ProductResponseDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category.Name));
    }
}
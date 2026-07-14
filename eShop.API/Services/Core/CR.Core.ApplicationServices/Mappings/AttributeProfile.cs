using AutoMapper;
using CR.Core.Dtos.AttributeModule;

namespace CR.Core.ApplicationServices.Mappings;

public class AttributeProfile : Profile
{
    public AttributeProfile()
    {
        CreateMap<AttributeRequest, CR.Core.Domain.Catalog.Attribute>()
            .ForMember(dest => dest.AttributeType, opt => opt.MapFrom(src => src.AttributeType.ToString()));
            
        CreateMap<CR.Core.Domain.Catalog.Attribute, AttributeResponseDto>();
    }
}

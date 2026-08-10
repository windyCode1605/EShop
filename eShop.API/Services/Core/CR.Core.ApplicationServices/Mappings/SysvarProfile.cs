using AutoMapper;
using CR.Core.Domain.SysVar;
using CR.Core.Dto.SysvarModule;

namespace CR.Core.ApplicationServices.Mappings;

public class SysvarProfile : Profile
{
    public SysvarProfile()
    {
        CreateMap<SysVar, SysvarResponsDto>();
        CreateMap<SysvarResponsDto, SysVar>();
    }
}

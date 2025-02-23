using AutoMapper;
using Microservices.PlatformService.Dtos;
using Microservices.PlatformService.Models;

namespace Microservices.PlatformService.Profiles;

public class PlatformsProfile : Profile
{
    public PlatformsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<PlatformCreateDto, Platform>();
        CreateMap<PlatformReadDto, PlatformPublishedDto>();
        CreateMap<Platform, GrpcPlatformModel>()
            .ForMember(x
                => x.PlatformId, opt
                => opt.MapFrom(y => y.Id));
    }
}
using AutoMapper;
using Microservices.CommandsService.Dtos;
using Microservices.CommandsService.Models;
using Microservices.PlatformService;

namespace Microservices.CommandsService.Profiles;

public class CommandsProfile : Profile
{
    public CommandsProfile()
    {
        CreateMap<Platform, PlatformReadDto>();
        CreateMap<Command, CommandReadDto>();
        CreateMap<CommandCreateDto, Command>();
        CreateMap<PlatformPublishedDto, Platform>()
            .ForMember(dest
                => dest.ExternalId, opt
                => opt.MapFrom(src 
                    => src.Id));
        CreateMap<GrpcPlatformModel, Platform>()
            .ForMember(x
                => x.ExternalId, opt
                => opt.MapFrom(x
                    => x.PlatformId))
            .ForMember(x
                => x.Commands, opt
                => opt.Ignore());
    }
}
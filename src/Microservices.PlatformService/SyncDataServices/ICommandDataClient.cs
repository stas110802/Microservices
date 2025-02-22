using Microservices.PlatformService.Dtos;
using Microservices.PlatformService.Models;

namespace Microservices.PlatformService.SyncDataServices;

public interface ICommandDataClient
{
    Task SendPlatformToCommand(PlatformReadDto platform);
}
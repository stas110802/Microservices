using Microservices.PlatformService.Dtos;
using Microservices.PlatformService.Models;

namespace Microservices.PlatformService.AsyncDataServices;

public interface IMessageBusClient : IAsyncDisposable
{
    Task PublishNewPlatformAsync(PlatformPublishedDto platform);
    Task CreateConnectAsync();
}
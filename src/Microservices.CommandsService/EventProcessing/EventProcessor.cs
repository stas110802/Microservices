using System.Text.Json;
using AutoMapper;
using Microservices.CommandsService.Data;
using Microservices.CommandsService.Dtos;
using Microservices.CommandsService.Models;
using Microservices.CommandsService.Types;

namespace Microservices.CommandsService.EventProcessing;

public class EventProcessor : IEventProcessor
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory serviceScopeFactory, IMapper mapper)
    {
        _scopeFactory = serviceScopeFactory;
        _mapper = mapper;
    }
    
    public void ProcessEvent(string message)
    {
        var eventType = GetEventType(message);
        switch (eventType)
        {
            case EventType.PlatformPublished:
                AddPlatform(message);
                break;
        }
    }

    private EventType GetEventType(string message)
    {
        Console.WriteLine("Determining Event");
        var eventType = JsonSerializer.Deserialize<GenericEventDto>(message);
        
        switch (eventType?.Event)
        {
            case "Platform Published":
                Console.WriteLine("Platform Published Event detected");
                return EventType.PlatformPublished;
            default:
                Console.WriteLine("Unknown Event Type");
                return EventType.Undetermined;
        }
    }

    private void AddPlatform(string platformPublishedMsg)
    {
        using var scope = _scopeFactory.CreateScope();
        var repo = scope.ServiceProvider.GetService<ICommandRepo>();
        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMsg);
        try
        {
            var plat = _mapper.Map<Platform>(platformPublishedDto);
            if (!repo.IsExternalPlatformExist(plat.ExternalId))
            {
                repo.CreatePlatform(plat);
                repo.SaveChanges();
                Console.WriteLine("Platform added");
            }
            else
            {
                Console.WriteLine("Platform already exists");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}
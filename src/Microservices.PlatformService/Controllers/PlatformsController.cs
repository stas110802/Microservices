using AutoMapper;
using Microservices.PlatformService.AsyncDataServices;
using Microservices.PlatformService.Data;
using Microservices.PlatformService.Dtos;
using Microservices.PlatformService.Models;
using Microservices.PlatformService.SyncDataServices;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.PlatformService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PlatformsController : Controller
{
    private readonly IPlatformRepo _repository;
    private readonly IMapper _mapper;
    private readonly ICommandDataClient _commandDataClient;
    private readonly IMessageBusClient _messageBusClient;

    public PlatformsController(
        IPlatformRepo repository,
        IMapper mapper,
        ICommandDataClient commandDataClient,
        IMessageBusClient messageBus)
    {
        _repository = repository;
        _mapper = mapper;
        _commandDataClient = commandDataClient;
        _messageBusClient = messageBus;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        var platforms = _repository.GetAllPlatforms();
        foreach (var platform in platforms)
        {
            Console.WriteLine(platform.Name);
        }

        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(platforms));
    }

    [HttpGet("{id}", Name = "GetPlatformById")]
    public ActionResult<PlatformReadDto> GetPlatformById(int id)
    {
        var platform = _repository.GetPlatformById(id);
        if (platform != null)
            return Ok(_mapper.Map<PlatformReadDto>(platform));
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<PlatformReadDto>> CreatePlatform(PlatformCreateDto platformCreateDto)
    {
        var platform = _mapper.Map<Platform>(platformCreateDto);
        _repository.CreatePlatform(platform);
        _repository.SaveChanges();

        var platformReadDto = _mapper.Map<PlatformReadDto>(platform);
    
        // sync msg
        try
        {
            await _commandDataClient.SendPlatformToCommand(platformReadDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
        
        // async msg
        try
        {
            var platformPublishedDto = _mapper.Map<PlatformPublishedDto>(platformReadDto);
            platformPublishedDto.Event = "Platform Published";
            await _messageBusClient.PublishNewPlatformAsync(platformPublishedDto);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return CreatedAtRoute(nameof(GetPlatformById), new { id = platformReadDto.Id }, platformReadDto);
    }
}
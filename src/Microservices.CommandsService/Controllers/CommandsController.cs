using AutoMapper;
using Microservices.CommandsService.Data;
using Microservices.CommandsService.Dtos;
using Microservices.CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.CommandsService.Controllers;
    
[Route("api/c/platforms/{platformId}/[controller]")]
[ApiController]
public class CommandsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public CommandsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }
    
    [HttpGet]
    public ActionResult<IEnumerable<CommandReadDto>> GetCommandsForPlatform(int platformId)
    {
        Console.WriteLine($"Try get commands for Platform... {platformId}");
        var check = _repository.IsPlatformExist(platformId);
        if(!check)
            return NotFound();
        
        var commands = _repository.GetCommandsForPlatforms(platformId);
        
        return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commands));
    }
    
    [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
    public ActionResult<CommandReadDto> GetCommandForPlatform(int platformId, int commandId)
    {
        Console.WriteLine($"Try get 1 command for Platform... {platformId} and {commandId}");
        var check = _repository.IsPlatformExist(platformId);
        var command = _repository.GetCommand(platformId, commandId);
        
        if(!check || command == null)
            return NotFound();
        
        return Ok(_mapper.Map<CommandReadDto>(command));
    }
    
    [HttpPost]
    public ActionResult<CommandReadDto> CreateCommandForPlatform(int platformId, CommandCreateDto commandDto)
    {
        Console.WriteLine($"Try create command for Platform... {platformId}");
        var check = _repository.IsPlatformExist(platformId);
        if(!check)
            return NotFound();
        var command = _mapper.Map<Command>(commandDto);
        _repository.CreateCommand(platformId, command);
        _repository.SaveChanges();
        
        return CreatedAtRoute(nameof(GetCommandForPlatform), 
            new { platformId = platformId, commandId = command.Id }, _mapper.Map<CommandReadDto>(command));
    }
}
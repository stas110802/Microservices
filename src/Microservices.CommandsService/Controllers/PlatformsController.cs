using AutoMapper;
using Microservices.CommandsService.Data;
using Microservices.CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Microservices.CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    private readonly ICommandRepo _repository;
    private readonly IMapper _mapper;

    public PlatformsController(ICommandRepo repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    [HttpGet]
    public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
    {
        Console.WriteLine("Getting platforms");
        return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_repository.GetAllPlatforms()));
    }
    
    [HttpPost]
    public ActionResult TestConnection()
    {
        return Ok("Testing connection");
    }
}
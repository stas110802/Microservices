using Microsoft.AspNetCore.Mvc;

namespace Microservices.CommandsService.Controllers;

[Route("api/c/[controller]")]
[ApiController]
public class PlatformsController : ControllerBase
{
    public PlatformsController()
    {
        
    }

    [HttpPost]
    public ActionResult TestConnection()
    {
        return Ok("Testing connection");
    }
}
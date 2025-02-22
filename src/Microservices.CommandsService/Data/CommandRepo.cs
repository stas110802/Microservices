using Microservices.CommandsService.Models;

namespace Microservices.CommandsService.Data;

public class CommandRepo : ICommandRepo
{
    private readonly AppDbContext _context;

    public CommandRepo(AppDbContext context)
    {
        _context = context;
    }
    
    public bool SaveChanges()
    {
       return _context.SaveChanges() >= 0;
    }

    public IEnumerable<Platform> GetAllPlatforms()
    {
        return _context.Platforms.ToList();
    }

    public void CreatePlatform(Platform platform)
    {
        _context.Platforms.Add(platform);
    }

    public bool IsPlatformExist(int id)
    {
        return _context.Platforms
            .Any(p => p.Id == id);
    }

    public bool IsExternalPlatformExist(int externalPlatformId)
    {
        return _context.Platforms
            .Any(p => p.ExternalId == externalPlatformId);
    }

    public IEnumerable<Command> GetCommandsForPlatforms(int platformId)
    {
        return _context.Commands
            .Where(x => x.PlatformId == platformId)
            .OrderBy(x => x.Platform.Name);
    }

    public Command? GetCommand(int platformId, int commandId)
    {
        return _context.Commands
            .FirstOrDefault(x => x.PlatformId == platformId && x.Id == commandId);
    }

    public void CreateCommand(int platformId, Command command)
    {
        command.PlatformId = platformId;
        _context.Commands.Add(command);
    }
}
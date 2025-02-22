using Microservices.CommandsService.Models;

namespace Microservices.CommandsService.Data;

public interface ICommandRepo
{
    bool SaveChanges();
    
    IEnumerable<Platform> GetAllPlatforms();
    void CreatePlatform(Platform platform);
    bool IsPlatformExist(int id);
    bool IsExternalPlatformExist(int externalPlatformId);
    
    IEnumerable<Command> GetCommandsForPlatforms(int platformId);
    Command? GetCommand(int platformId, int commandId);
    void CreateCommand(int platformId, Command command);
}
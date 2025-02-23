using Microservices.CommandsService.Models;
using Microservices.CommandsService.SyncDataServices.Grpc;

namespace Microservices.CommandsService.Data;

public class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var grpcClient = serviceScope.ServiceProvider.GetService<IPlatformDataClient>();
        var platforms = grpcClient.ReturnAllPlatforms();
        var repo = serviceScope.ServiceProvider.GetService<ICommandRepo>();
        SeedData(repo, platforms);
    }

    private static void SeedData(ICommandRepo repo, IEnumerable<Platform> platforms)
    {
        Console.WriteLine("Seeding new platforms...");
        foreach (var platform in platforms)
        {
            if(repo.IsExternalPlatformExist(platform.ExternalId))
                continue;
            repo.CreatePlatform(platform);
            repo.SaveChanges();
        }
    }
}
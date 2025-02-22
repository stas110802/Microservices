using Microservices.PlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.PlatformService.Data;

public static class PrepDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProduction)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        SetData(serviceScope.ServiceProvider.GetRequiredService<AppDbContext>(), isProduction);
    }

    private static void SetData(AppDbContext context, bool isProduction)
    {
        if (isProduction)
        {
            Console.WriteLine("Attempting to apply migrations...");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception e)
            {
                Console.WriteLine("Cannot migrate database, see inner exception.");
                Console.WriteLine(e.Message);
            }
            
        }
        if (context.Platforms.Any() is false)
        {
            Console.WriteLine("Seeding data...");
            context.Platforms.AddRange(
                new Platform { Name = "DotNet", Publisher = "Microsoft", Cost = "Free" },
                new Platform { Name = "SQL Server", Publisher = "Microsoft", Cost = "Free" },
                new Platform { Name = "Kubernetes", Publisher = "Cloud Native Stuff", Cost = "Free" });
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("We already have data");
        }
    }
}
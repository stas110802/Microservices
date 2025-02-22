using Microservices.PlatformService.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservices.PlatformService.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<Platform> Platforms { get; set; }
}
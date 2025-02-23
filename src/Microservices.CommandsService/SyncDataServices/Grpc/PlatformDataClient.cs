using System.Collections.Immutable;
using AutoMapper;
using Grpc.Net.Client;
using Microservices.CommandsService.Models;
using Microservices.PlatformService;

namespace Microservices.CommandsService.SyncDataServices.Grpc;

public class PlatformDataClient : IPlatformDataClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public PlatformDataClient(IConfiguration configuration, IMapper mapper)
    {
        _configuration = configuration;
        _mapper = mapper;
    }
    
    public IEnumerable<Platform> ReturnAllPlatforms()
    {
        var address = _configuration["GrpcPlatform"];
        if(string.IsNullOrWhiteSpace(address))
            throw new InvalidOperationException("No grpc platform configured in appsettings.json");
        
        Console.WriteLine($"Calling Grpc Service {address}");
        var channel = GrpcChannel.ForAddress(address);
        var client = new GrpcPlatform.GrpcPlatformClient(channel);
        var request = new GetAllRequest();
        
        try
        {
            var reply = client.GetAllPlatforms(request);
            return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Couldn't call Grpc Server {e.Message}");
        }
        
        return [];
    }
}
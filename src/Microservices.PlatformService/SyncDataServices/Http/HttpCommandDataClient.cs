﻿using System.Text;
using System.Text.Json;
using Microservices.PlatformService.Dtos;

namespace Microservices.PlatformService.SyncDataServices.Http;

public class HttpCommandDataClient : ICommandDataClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public HttpCommandDataClient(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }
    
    public async Task SendPlatformToCommand(PlatformReadDto platform)
    {
        var httpContent = new StringContent(
            JsonSerializer.Serialize(platform), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);
        
        if(response.IsSuccessStatusCode)
            Console.WriteLine("Success");
        else
            Console.WriteLine("Failed");
    }
}
using System.Text;
using System.Text.Json;
using Microservices.PlatformService.Dtos;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microservices.PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private IConnection _connection;
    private IChannel _chanel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task PublishNewPlatformAsync(PlatformPublishedDto platform)
    {
        var message = JsonSerializer.Serialize(platform);
        if (_connection.IsOpen)
        {
            Console.WriteLine($"Publishing new platform: {message}");
            await SendMessage(message);
        }
        else
        {
            Console.WriteLine("RabbitMQ connection is closed.");
        }
    }

    public async Task CreateConnectAsync()
    {
        var hostName = _configuration["RabbitMQHost"];
        var port = int.Parse(_configuration["RabbitMQPort"]);

        var factory = new ConnectionFactory
        {
            HostName = hostName,
            Port = port
        };
        try
        {
            _connection = await factory.CreateConnectionAsync();
            _chanel = await _connection.CreateChannelAsync();
            await _chanel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
            Console.WriteLine("Connected to RabbitMQ MessageBus");
        }
        catch (Exception e)
        {
            Console.WriteLine("Could not connect to RabbitMQ. Please check your connection string and try again.");
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_chanel.IsOpen)
        {
            await _chanel.CloseAsync();
            await _connection.CloseAsync();
        }
    }
    
    private async Task SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        await _chanel.BasicPublishAsync(
            exchange: "trigger",
            routingKey: "",
            body: body);
        Console.WriteLine($"Published message: {message}");
    }
}
using System.Text;
using Microservices.CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Microservices.CommandsService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService, IAsyncDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection;
    private IChannel _channel;
    private string _queueName;
    private bool _isInitialized;
    
    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;
        _isInitialized = false;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_isInitialized)
            await InitializeRabbitMq();
        stoppingToken.ThrowIfCancellationRequested();
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (sender, @event) =>
        {
            Console.WriteLine("Event received!");
            var body = @event.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            _eventProcessor.ProcessEvent(message);
            await Task.Delay(300, stoppingToken);
        };
        await _channel.BasicConsumeAsync(_queueName, true, consumer, cancellationToken: stoppingToken);
    }

    private async Task InitializeRabbitMq()
    {
        
        var factory = new ConnectionFactory
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"]),
        };
        
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();
        await _channel.ExchangeDeclareAsync(exchange: "trigger", type: ExchangeType.Fanout);
        _queueName = (await _channel.QueueDeclareAsync()).QueueName;
        await _channel.QueueBindAsync(queue: _queueName,
            exchange: "trigger",
            routingKey: "");
        
        Console.WriteLine("Listening on Message Bus...");
        _connection.ConnectionShutdownAsync += async (sender, @event) =>
        {
            Console.WriteLine("Connection Shutdown");
            await Task.Delay(300);
        };
    }

    public async ValueTask DisposeAsync()
    {
        if (_channel.IsOpen)
        {
            await _channel.CloseAsync();
            await _connection.CloseAsync();
        }
        base.Dispose();
    }
}
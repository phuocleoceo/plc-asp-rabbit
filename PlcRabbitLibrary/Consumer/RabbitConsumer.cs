using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlcRabbitLibrary.Consumer;

public class RabbitConsumer<T> : IHostedService, IDisposable
{
    private readonly RabbitConsumerConfig _rabbitConsumerConfig;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<RabbitConsumer<T>> _logger;
    private IRabbitConsumerHandler<T> _rabbitConsumerHandler;
    private IConnection _connection;
    private IModel _channel;

    public RabbitConsumer(
        IOptions<RabbitConsumerConfig> rabbitConsumerConfig,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitConsumer<T>> logger
    )
    {
        _rabbitConsumerConfig = rabbitConsumerConfig.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        InitRabbitConsumer();
    }

    private void InitRabbitConsumer()
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = _rabbitConsumerConfig.HostName,
            Port = _rabbitConsumerConfig.Port
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: _rabbitConsumerConfig.ExchangeName,
            type: ExchangeType.Topic
        );

        _channel.QueueDeclare(
            queue: _rabbitConsumerConfig.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        foreach (string routingKey in _rabbitConsumerConfig.RoutingKeys)
        {
            _channel.QueueBind(
                queue: _rabbitConsumerConfig.QueueName,
                exchange: _rabbitConsumerConfig.ExchangeName,
                routingKey: routingKey,
                arguments: null
            );
        }

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Task.Run(
            () =>
            {
                EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
                consumer.Received += OnConsumerReceived;
                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerCancelled;

                _channel.BasicConsume(
                    queue: _rabbitConsumerConfig.QueueName,
                    autoAck: false,
                    consumer: consumer
                );
            },
            cancellationToken
        );

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void OnConsumerReceived(object sender, BasicDeliverEventArgs e)
    {
        _logger.LogInformation($"RabbitMQ Consumer Key: {e.RoutingKey}");

        using IServiceScope scope = _serviceScopeFactory.CreateScope();

        _rabbitConsumerHandler = scope
            .ServiceProvider
            .GetRequiredService<IRabbitConsumerHandler<T>>();

        _rabbitConsumerHandler.HandleAsync(RabbitDeserializer<T>.Deserialize(e.Body.ToArray()));

        _channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: false);
    }

    private void OnConsumerCancelled(object sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("RabbitMQ Consumer Cancelled");
    }

    private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("RabbitMQ Consumer Unregistered");
    }

    private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
    {
        _logger.LogInformation("RabbitMQ Consumer Registered");
    }

    private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("RabbitMQ Consumer Shutdown");
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("RabbitMQ Consumer Connection Shutdown");
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}

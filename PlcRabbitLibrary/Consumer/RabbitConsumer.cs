using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlcRabbitLibrary.Consumer;

public class RabbitConsumer<T> : IHostedService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly RabbitConsumerConfig _rabbitConsumerConfig;
    private readonly ILogger<RabbitConsumer<T>> _logger;
    private readonly IModel _channel;

    private IRabbitConsumerHandler<T> _rabbitConsumerHandler;

    public RabbitConsumer(
        IOptions<RabbitConsumerConfig> rabbitConsumerConfig,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<RabbitConsumer<T>> logger,
        IModel channel
    )
    {
        _rabbitConsumerConfig = rabbitConsumerConfig.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _channel = channel;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Task.Run(
            () =>
            {
                using IServiceScope scope = _serviceScopeFactory.CreateScope();

                _rabbitConsumerHandler = scope
                    .ServiceProvider
                    .GetRequiredService<IRabbitConsumerHandler<T>>();

                EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

                consumer.Received += OnConsumerReceived;
                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerCancelled;

                _channel.BasicConsume(
                    queue: _rabbitConsumerHandler.QueueName,
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
}

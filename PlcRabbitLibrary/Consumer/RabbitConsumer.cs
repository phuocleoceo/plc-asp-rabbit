using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace PlcRabbitLibrary.Consumer;

public class RabbitConsumer<T>(
    IServiceScopeFactory serviceScopeFactory,
    IOptions<RabbitMQConfig> rabbitMqConfig,
    ILogger<RabbitConsumer<T>> logger,
    IModel channel
) : IHostedService
{
    private readonly RabbitConsumerConfig _rabbitConsumerConfig = rabbitMqConfig.Value.Consumer;
    private IRabbitConsumerHandler<T> _rabbitConsumerHandler;

    public Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Task.Run(
            () =>
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();

                _rabbitConsumerHandler = scope.ServiceProvider.GetRequiredService<
                    IRabbitConsumerHandler<T>
                >();

                EventingBasicConsumer consumer = new(channel);

                consumer.Received += OnConsumerReceived;
                consumer.Shutdown += OnConsumerShutdown;
                consumer.Registered += OnConsumerRegistered;
                consumer.Unregistered += OnConsumerUnregistered;
                consumer.ConsumerCancelled += OnConsumerCancelled;

                channel.BasicConsume(
                    queue: _rabbitConsumerHandler.QueueName,
                    autoAck: _rabbitConsumerConfig.AutoAck,
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
        _rabbitConsumerHandler.HandleAsync(RabbitDeserializer<T>.Deserialize(e.Body.ToArray()));
        channel.BasicAck(deliveryTag: e.DeliveryTag, multiple: _rabbitConsumerConfig.AckMultiple);
    }

    private void OnConsumerCancelled(object sender, ConsumerEventArgs e)
    {
        logger.LogInformation("RabbitMQ Consumer Cancelled");
    }

    private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
    {
        logger.LogInformation("RabbitMQ Consumer Unregistered");
    }

    private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
    {
        logger.LogInformation("RabbitMQ Consumer Registered");
    }

    private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
    {
        logger.LogInformation("RabbitMQ Consumer Shutdown");
    }
}

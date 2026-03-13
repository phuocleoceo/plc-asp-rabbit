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
    IChannel channel
) : IHostedService
{
    private readonly RabbitConsumerConfig _rabbitConsumerConfig = rabbitMqConfig.Value.Consumer;
    private IRabbitConsumerHandler<T> _rabbitConsumerHandler;

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using IServiceScope scope = serviceScopeFactory.CreateScope();

        _rabbitConsumerHandler = scope.ServiceProvider.GetRequiredService<
            IRabbitConsumerHandler<T>
        >();

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += OnConsumerReceived;
        consumer.RegisteredAsync += OnConsumerRegistered;
        consumer.UnregisteredAsync += OnConsumerUnregistered;
        consumer.ShutdownAsync += OnConsumerShutdown;

        await channel.BasicConsumeAsync(
            queue: _rabbitConsumerHandler.QueueName,
            autoAck: _rabbitConsumerConfig.AutoAck,
            consumer: consumer,
            cancellationToken: cancellationToken
        );
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async Task OnConsumerReceived(object sender, BasicDeliverEventArgs e)
    {
        try
        {
            await _rabbitConsumerHandler.HandleAsync(
                RabbitDeserializer<T>.Deserialize(e.Body.ToArray())
            );

            await channel.BasicAckAsync(
                deliveryTag: e.DeliveryTag,
                multiple: _rabbitConsumerConfig.AckMultiple
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error handling RabbitMQ message");
        }
    }

    private Task OnConsumerUnregistered(object sender, ConsumerEventArgs e)
    {
        logger.LogInformation("RabbitMQ Consumer Unregistered");
        return Task.CompletedTask;
    }

    private Task OnConsumerRegistered(object sender, ConsumerEventArgs e)
    {
        logger.LogInformation("RabbitMQ Consumer Registered");
        return Task.CompletedTask;
    }

    private Task OnConsumerShutdown(object sender, ShutdownEventArgs e)
    {
        logger.LogInformation("RabbitMQ Consumer Shutdown");
        return Task.CompletedTask;
    }
}

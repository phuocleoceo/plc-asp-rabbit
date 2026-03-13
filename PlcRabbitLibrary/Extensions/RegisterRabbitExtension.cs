using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Extensions;

public static class RegisterRabbitExtension
{
    public static IServiceCollection ConfigureRabbitConnection(this IServiceCollection services)
    {
        services.AddSingleton(serviceProvider =>
        {
            RabbitMQConfig rabbitMqConfig = serviceProvider
                .GetRequiredService<IOptions<RabbitMQConfig>>()
                .Value;

            ConnectionFactory factory = rabbitMqConfig.Connection;
            IConnection connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            IChannel channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

            foreach (RabbitExchangeConfig exchangeConfig in rabbitMqConfig.Exchanges)
            {
                channel
                    .ExchangeDeclareAsync(
                        exchange: exchangeConfig.ExchangeName,
                        type: ExchangeType.Topic
                    )
                    .GetAwaiter()
                    .GetResult();

                foreach (RabbitQueueConfig queueConfig in exchangeConfig.Queues)
                {
                    channel
                        .QueueDeclareAsync(
                            queue: queueConfig.QueueName,
                            durable: queueConfig.Durable,
                            exclusive: queueConfig.Exclusive,
                            autoDelete: queueConfig.AutoDelete,
                            arguments: queueConfig.Arguments
                        )
                        .GetAwaiter()
                        .GetResult();

                    foreach (RabbitRoutingKeyConfig routingKeyConfig in queueConfig.RoutingKeys)
                    {
                        channel
                            .QueueBindAsync(
                                queue: queueConfig.QueueName,
                                exchange: exchangeConfig.ExchangeName,
                                routingKey: routingKeyConfig.KeyName,
                                arguments: routingKeyConfig.Arguments
                            )
                            .GetAwaiter()
                            .GetResult();
                    }
                }
            }

            RabbitQosConfig rabbitQosConfig = rabbitMqConfig.Qos;

            channel
                .BasicQosAsync(
                    prefetchSize: rabbitQosConfig.PrefetchSize,
                    prefetchCount: rabbitQosConfig.PrefetchCount,
                    global: rabbitQosConfig.Global
                )
                .GetAwaiter()
                .GetResult();

            return channel;
        });

        return services;
    }
}

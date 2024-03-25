using Microsoft.Extensions.DependencyInjection;
using PlcRabbitLibrary.Configuration;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Extensions;

public static class RegisterRabbitExtension
{
    public static IServiceCollection ConfigureRabbitConnection(
        this IServiceCollection services,
        RabbitMQConfig rabbitMqConfig
    )
    {
        ConnectionFactory factory =
            new() { HostName = rabbitMqConfig.HostName, Port = rabbitMqConfig.Port };

        IConnection connection = factory.CreateConnection();

        IModel channel = connection.CreateModel();

        foreach (RabbitExchangeConfig exchangeConfig in rabbitMqConfig.ExchangeConfigs)
        {
            channel.ExchangeDeclare(
                exchange: exchangeConfig.ExchangeName,
                type: ExchangeType.Topic
            );

            foreach (RabbitQueueConfig queueConfig in exchangeConfig.QueueConfigs)
            {
                channel.QueueDeclare(
                    queue: queueConfig.QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                foreach (string routingKey in queueConfig.RoutingKeys)
                {
                    channel.QueueBind(
                        queue: queueConfig.QueueName,
                        exchange: exchangeConfig.ExchangeName,
                        routingKey: routingKey,
                        arguments: null
                    );
                }
            }
        }

        channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        services.AddSingleton(connection);
        services.AddSingleton(channel);
        return services;
    }

    private static void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        // _logger.LogInformation("RabbitMQ Consumer Connection Shutdown");
    }
}

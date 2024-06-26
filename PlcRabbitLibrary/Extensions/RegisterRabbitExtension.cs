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
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();

            foreach (RabbitExchangeConfig exchangeConfig in rabbitMqConfig.Exchanges)
            {
                channel.ExchangeDeclare(
                    exchange: exchangeConfig.ExchangeName,
                    type: ExchangeType.Topic
                );

                foreach (RabbitQueueConfig queueConfig in exchangeConfig.Queues)
                {
                    channel.QueueDeclare(
                        queue: queueConfig.QueueName,
                        durable: queueConfig.Durable,
                        exclusive: queueConfig.Exclusive,
                        autoDelete: queueConfig.AutoDelete,
                        arguments: queueConfig.Arguments
                    );

                    foreach (RabbitRoutingKeyConfig routingKeyConfig in queueConfig.RoutingKeys)
                    {
                        channel.QueueBind(
                            queue: queueConfig.QueueName,
                            exchange: exchangeConfig.ExchangeName,
                            routingKey: routingKeyConfig.KeyName,
                            arguments: routingKeyConfig.Arguments
                        );
                    }
                }
            }

            RabbitQosConfig rabbitQosConfig = rabbitMqConfig.Qos;

            channel.BasicQos(
                prefetchSize: rabbitQosConfig.PrefetchSize,
                prefetchCount: rabbitQosConfig.PrefetchCount,
                global: rabbitQosConfig.Global
            );

            connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
            return channel;
        });

        return services;
    }

    private static void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        // _logger.LogInformation("RabbitMQ Consumer Connection Shutdown");
    }
}

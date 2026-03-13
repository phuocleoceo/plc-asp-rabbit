using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Producer;

public class RabbitProducer<T>(IOptions<RabbitMQConfig> rabbitMqConfig, IChannel channel)
    : IRabbitProducer<T>
{
    private readonly RabbitProducerConfig _rabbitProducerConfig = rabbitMqConfig.Value.Producer;

    public async Task ProduceAsync(string exchange, string routingKey, T data)
    {
        await channel.BasicPublishAsync(
            exchange: exchange,
            routingKey: routingKey,
            body: RabbitSerializer<T>.Serialize(data)
        );
    }
}

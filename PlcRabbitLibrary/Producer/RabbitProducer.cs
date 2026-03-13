using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Producer;

public class RabbitProducer<T>(IOptions<RabbitMQConfig> rabbitMqConfig, IModel channel)
    : IRabbitProducer<T>
{
    private readonly RabbitProducerConfig _rabbitProducerConfig = rabbitMqConfig.Value.Producer;

    public async Task ProduceAsync(string exchange, string routingKey, T data)
    {
        channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            body: RabbitSerializer<T>.Serialize(data),
            basicProperties: null
        );
        await Task.CompletedTask;
    }
}

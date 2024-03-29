using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Producer;

public class RabbitProducer<T> : IRabbitProducer<T>
{
    private readonly RabbitProducerConfig _rabbitProducerConfig;

    private readonly ILogger<RabbitProducer<T>> _logger;
    private readonly IModel _channel;

    public RabbitProducer(
        IOptions<RabbitMQConfig> rabbitMqConfig,
        ILogger<RabbitProducer<T>> logger,
        IModel channel
    )
    {
        _rabbitProducerConfig = rabbitMqConfig.Value.Producer;
        _channel = channel;
        _logger = logger;
    }

    public async Task ProduceAsync(string exchange, string routingKey, T data)
    {
        _channel.BasicPublish(
            exchange: exchange,
            routingKey: routingKey,
            body: RabbitSerializer<T>.Serialize(data),
            basicProperties: null
        );
        await Task.CompletedTask;
    }
}

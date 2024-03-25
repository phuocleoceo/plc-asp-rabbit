using Microsoft.Extensions.Logging;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Producer;

public class RabbitProducer<T> : IRabbitProducer<T>
{
    private readonly ILogger<RabbitProducer<T>> _logger;
    private readonly IModel _channel;

    public RabbitProducer(IModel channel, ILogger<RabbitProducer<T>> logger)
    {
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

        _logger.LogInformation(
            $"Publish message {data} to routing key: {routingKey} with exchange: {exchange}"
        );
        await Task.CompletedTask;
    }
}

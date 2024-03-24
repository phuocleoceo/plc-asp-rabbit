using Microsoft.Extensions.Logging;
using PlcRabbitLibrary.Connection;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Producer;

public class RabbitProducer<T> : IRabbitProducer<T>
{
    private readonly RabbitConnection _rabbitConnection;
    private readonly ILogger<RabbitProducer<T>> _logger;

    public RabbitProducer(RabbitConnection rabbitConnection, ILogger<RabbitProducer<T>> logger)
    {
        _rabbitConnection = rabbitConnection;
        _logger = logger;
    }

    public async Task ProduceAsync(string exchange, string routingKey, T data)
    {
        _rabbitConnection
            .Channel
            .BasicPublish(
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

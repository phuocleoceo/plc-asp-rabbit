using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Data;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Producer;

public class RabbitProducer<T> : IRabbitProducer<T>, IDisposable
{
    private readonly RabbitProducerConfig _rabbitProducerConfig;
    private readonly ILogger<RabbitProducer<T>> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public RabbitProducer(
        ILogger<RabbitProducer<T>> logger,
        IOptions<RabbitProducerConfig> rabbitProducerConfig
    )
    {
        _logger = logger;
        _rabbitProducerConfig = rabbitProducerConfig.Value;

        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = _rabbitProducerConfig.HostName,
            Port = _rabbitProducerConfig.Port
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("RabbitMQ Shutdown");
    }

    public void Dispose()
    {
        _connection.Close();
        _channel.Close();
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

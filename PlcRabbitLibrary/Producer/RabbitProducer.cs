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
    private IConnection _connection;
    private IModel _channel;

    public RabbitProducer(
        IOptions<RabbitProducerConfig> rabbitProducerConfig,
        ILogger<RabbitProducer<T>> logger
    )
    {
        _rabbitProducerConfig = rabbitProducerConfig.Value;
        _logger = logger;
        InitRabbitProducer();
    }

    private void InitRabbitProducer()
    {
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
        _logger.LogInformation("RabbitMQ Producer Connection Shutdown");
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

    public void Dispose()
    {
        _connection.Close();
        _channel.Close();
    }
}

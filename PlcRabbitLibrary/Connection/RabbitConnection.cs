using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlcRabbitLibrary.Configuration;
using RabbitMQ.Client;

namespace PlcRabbitLibrary.Connection;

public class RabbitConnection : IDisposable
{
    private readonly ILogger<RabbitConnection> _logger;
    private readonly RabbitMQConfig _rabbitMQConfig;
    private IConnection _connection;
    private IModel _channel;

    public IModel Channel => _channel;

    public RabbitConnection(
        IOptions<RabbitMQConfig> rabbitMQConfig,
        ILogger<RabbitConnection> logger
    )
    {
        _rabbitMQConfig = rabbitMQConfig.Value;
        _logger = logger;
        InitRabbit();
    }

    private void InitRabbit()
    {
        ConnectionFactory factory = new()
        {
            HostName = _rabbitMQConfig.HostName,
            Port = _rabbitMQConfig.Port
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        foreach (RabbitExchangeConfig exchangeConfig in _rabbitMQConfig.ExchangeConfigs)
        {
            _channel.ExchangeDeclare(exchange: exchangeConfig.ExchangeName, type: ExchangeType.Topic);

            foreach (RabbitQueueConfig queueConfig in exchangeConfig.QueueConfigs)
            {
                _channel.QueueDeclare(
                    queue: queueConfig.QueueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );

                foreach (string routingKey in queueConfig.RoutingKeys)
                {
                    _channel.QueueBind(
                        queue: queueConfig.QueueName,
                        exchange: exchangeConfig.ExchangeName,
                        routingKey: routingKey,
                        arguments: null
                    );
                }
            }
        }

        _channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        _logger.LogInformation("RabbitMQ Consumer Connection Shutdown");
    }

    public void Dispose()
    {
        _channel.Close();
        _connection.Close();
    }
}

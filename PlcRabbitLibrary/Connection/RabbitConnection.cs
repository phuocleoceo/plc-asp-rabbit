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
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = _rabbitMQConfig.HostName,
            Port = _rabbitMQConfig.Port
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(exchange: _rabbitMQConfig.ExchangeName, type: ExchangeType.Topic);

        foreach (RabbitBindingConfig bindingConfig in _rabbitMQConfig.BindConfigs)
        {
            _channel.QueueDeclare(
                queue: bindingConfig.QueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            foreach (string routingKey in bindingConfig.RoutingKeys)
            {
                _channel.QueueBind(
                    queue: bindingConfig.QueueName,
                    exchange: _rabbitMQConfig.ExchangeName,
                    routingKey: routingKey,
                    arguments: null
                );
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

using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using PLCLib;

namespace PLCConsumer;

public class ProductConsumer : BackgroundService
{
    private IConnection _connection;
    private IModel _channel;

    public ProductConsumer()
    {
        InitRabbitMQs();
    }

    private void InitRabbitMQs()
    {
        ConnectionFactory factory = new ConnectionFactory
        {
            HostName = RabbitSettings.HostName
        };

        // create connection  
        _connection = factory.CreateConnection();

        // create channel
        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(RabbitSettings.ExchangeName, ExchangeType.Topic);
        _channel.QueueDeclare(RabbitSettings.QueueName, false, false, false, null);
        _channel.QueueBind(RabbitSettings.QueueName, RabbitSettings.ExchangeName, RabbitSettings.RoutingKey, null);
        _channel.BasicQos(0, 1, false);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, ea) =>
        {
            // received message  
            string content = Encoding.UTF8.GetString(ea.Body.ToArray());

            // handle the received message  
            HandleMessage(content);
            _channel.BasicAck(ea.DeliveryTag, false);
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume(RabbitSettings.QueueName, false, consumer);
        return Task.CompletedTask;
    }

    private void HandleMessage(string content)
    {
        Console.WriteLine($"consumer received {content}");
    }

    private void OnConsumerConsumerCancelled(object sender, ConsumerEventArgs e)
    {
    }

    private void OnConsumerUnregistered(object sender, ConsumerEventArgs e)
    {
    }

    private void OnConsumerRegistered(object sender, ConsumerEventArgs e)
    {
    }

    private void OnConsumerShutdown(object sender, ShutdownEventArgs e)
    {
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}
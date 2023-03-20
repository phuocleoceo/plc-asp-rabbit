using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using Newtonsoft.Json;
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
            HostName = RabbitSettings.HostName,
            Port = RabbitSettings.Port
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        _channel.ExchangeDeclare(
            exchange: RabbitSettings.ExchangeName,
            type: ExchangeType.Topic
        );

        _channel.QueueDeclare(
            queue: RabbitSettings.QueueName,
            durable: false,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );

        _channel.QueueBind(
            queue: RabbitSettings.QueueName,
            exchange: RabbitSettings.ExchangeName,
            routingKey: RabbitSettings.RoutingKey,
            arguments: null
        );

        _channel.BasicQos(
            prefetchSize: 0,
            prefetchCount: 1,
            global: false
        );

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    protected override Task ExecuteAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);
        consumer.Received += (_, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());

            HandleMessage(message);

            _channel.BasicAck(
                deliveryTag: ea.DeliveryTag,
                multiple: false
            );
        };

        consumer.Shutdown += OnConsumerShutdown;
        consumer.Registered += OnConsumerRegistered;
        consumer.Unregistered += OnConsumerUnregistered;
        consumer.ConsumerCancelled += OnConsumerConsumerCancelled;

        _channel.BasicConsume(
            queue: RabbitSettings.QueueName,
            autoAck: false,
            consumer: consumer
        );

        return Task.CompletedTask;
    }

    private void HandleMessage(string message)
    {
        Product product = JsonConvert.DeserializeObject<Product>(message);
        Console.WriteLine($"Consumer received {product}");
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
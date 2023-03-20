using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using PLCLib;

namespace PLCProducer;

public class ProductProducer : IProductProducer
{
    private IConnection _connection;
    private IModel _channel;

    public ProductProducer()
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

        // _channel.ExchangeDeclare(RabbitSettings.ExchangeName, type: ExchangeType.Topic);

        // _channel.QueueDeclare(queue: RabbitSettings.QueueName, false, false, false, null);

        // _channel.QueueBind(queue: RabbitSettings.QueueName, exchange: RabbitSettings.ExchangeName,
        //     routingKey: RabbitSettings.RoutingKey, null);

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
    }

    public void SendProductMessage(Product product)
    {
        //Serialize the message
        string json = JsonConvert.SerializeObject(product);
        byte[] body = Encoding.UTF8.GetBytes(json);

        //put the data on to the product queue
        _channel.BasicPublish(exchange: RabbitSettings.ExchangeName,
            routingKey: RabbitSettings.RoutingKey, body: body, basicProperties: null);
    }
}
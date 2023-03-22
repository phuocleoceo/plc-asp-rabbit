using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;
using PLCLib;

namespace PLCProducer;

public class ProductProducer : IProductProducer, IDisposable
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
            HostName = RabbitSettings.HostName,
            Port = RabbitSettings.Port
        };

        _connection = factory.CreateConnection();

        _channel = _connection.CreateModel();

        _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
    }

    public void SendProductMessage(Product product)
    {
        string json = JsonConvert.SerializeObject(product);
        byte[] body = Encoding.UTF8.GetBytes(json);

        _channel.BasicPublish(
            exchange: RabbitSettings.ExchangeName,
            routingKey: RabbitSettings.ProducerRoutingKey,
            body: body,
            basicProperties: null
        );
    }

    public void Dispose()
    {
        _connection.Close();
        _channel.Close();
    }
}
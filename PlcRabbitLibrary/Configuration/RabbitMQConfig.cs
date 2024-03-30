using RabbitMQ.Client;

namespace PlcRabbitLibrary.Configuration;

public class RabbitMQConfig
{
    public ConnectionFactory Connection { get; set; }
    public RabbitQosConfig Qos { get; set; }
    public RabbitProducerConfig Producer { get; set; }
    public RabbitConsumerConfig Consumer { get; set; }
    public List<RabbitExchangeConfig> Exchanges { get; set; }
}

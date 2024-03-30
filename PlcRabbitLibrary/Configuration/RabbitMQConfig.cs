namespace PlcRabbitLibrary.Configuration;

public class RabbitMQConfig
{
    public string HostName { get; set; } = "localhost";
    public string VirtualHost { get; set; } = "/";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public RabbitQosConfig Qos { get; set; }
    public RabbitProducerConfig Producer { get; set; }
    public RabbitConsumerConfig Consumer { get; set; }
    public List<RabbitExchangeConfig> Exchanges { get; set; }
}

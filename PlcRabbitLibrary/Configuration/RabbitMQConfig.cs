namespace PlcRabbitLibrary.Configuration;

public class RabbitMQConfig
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public int Port { get; set; } = 5672;
    public RabbitQosConfig Qos { get; set; }
    public RabbitProducerConfig Producer { get; set; }
    public RabbitConsumerConfig Consumer { get; set; }
    public List<RabbitExchangeConfig> Exchanges { get; set; }
}

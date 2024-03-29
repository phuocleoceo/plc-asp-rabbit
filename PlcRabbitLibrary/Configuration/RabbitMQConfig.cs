namespace PlcRabbitLibrary.Configuration;

public class RabbitMQConfig
{
    public string HostName { get; set; } = "localhost";
    public string UserName { get; set; } = "";
    public string Password { get; set; } = "";
    public int Port { get; set; } = 5672;
    public RabbitProducerConfig Producer { get; set; }
    public RabbitConsumerConfig Consumer { get; set; }
    public List<RabbitExchangeConfig> ExchangeConfigs { get; set; }
}

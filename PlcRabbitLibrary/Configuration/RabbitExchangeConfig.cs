namespace PlcRabbitLibrary.Configuration;

public class RabbitExchangeConfig
{
    public string ExchangeName { get; set; }
    public List<RabbitQueueConfig> QueueConfigs { get; set; }
}

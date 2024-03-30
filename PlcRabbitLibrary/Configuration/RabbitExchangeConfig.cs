namespace PlcRabbitLibrary.Configuration;

public class RabbitExchangeConfig
{
    public string ExchangeName { get; set; }
    public List<RabbitQueueConfig> Queues { get; set; }
}

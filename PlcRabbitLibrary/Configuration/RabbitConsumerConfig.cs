namespace PlcRabbitLibrary.Configuration;

public class RabbitConsumerConfig : RabbitCommonConfig
{
    public string ExchangeName { get; set; }
    public string QueueName { get; set; }
    public string[] RoutingKeys { get; set; }
}

namespace PlcRabbitLibrary.Configuration;

public class RabbitQueueConfig
{
    public string QueueName { get; set; }
    public string[] RoutingKeys { get; set; }

    public RabbitQueueConfig(string queueName, params string[] routingKeys)
    {
        QueueName = queueName;
        RoutingKeys = routingKeys;
    }
}

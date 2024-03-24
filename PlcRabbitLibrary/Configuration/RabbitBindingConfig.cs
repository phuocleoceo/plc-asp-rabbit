namespace PlcRabbitLibrary.Configuration;

public class RabbitBindingConfig
{
    public string QueueName { get; set; }
    public string[] RoutingKeys { get; set; }

    public RabbitBindingConfig(string queueName, params string[] routingKeys)
    {
        QueueName = queueName;
        RoutingKeys = routingKeys;
    }
}

namespace PlcRabbitLibrary.Configuration;

public class RabbitQueueConfig
{
    public string QueueName { get; set; }
    public bool Durable { get; set; } = false;
    public bool Exclusive { get; set; } = false;
    public bool AutoDelete { get; set; } = false;
    public Dictionary<string, object> Arguments { get; set; } = null;
    public List<RabbitRoutingKeyConfig> RoutingKeys { get; set; } = new();
}

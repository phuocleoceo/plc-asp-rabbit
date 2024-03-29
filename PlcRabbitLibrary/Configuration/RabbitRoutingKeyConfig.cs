namespace PlcRabbitLibrary.Configuration;

public class RabbitRoutingKeyConfig
{
    public string KeyName { get; set; }
    public Dictionary<string, object> Arguments { get; set; } = null;
}

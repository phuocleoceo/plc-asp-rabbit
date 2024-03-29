namespace PlcRabbitLibrary.Configuration;

public class RabbitConsumerConfig
{
    public bool AutoAck { get; set; } = false;
    public bool AckMultiple { get; set; } = false;
}

namespace PLCLib;

public static class RabbitSettings
{
    // General info
    public const bool RabbitEnable = true;

    public const string HostName = "localhost";

    public const int Port = 5672;

    // Consumer
    public const string ExchangeName = "plc.exchange";

    public const string QueueName = "plc.queue.product";

    public static readonly string[] ConsumerRoutingKey = { "plc.key.*", "plc2.key.*" };

    // Producer
    public const string ProducerRoutingKey = "plc.key.product";
}
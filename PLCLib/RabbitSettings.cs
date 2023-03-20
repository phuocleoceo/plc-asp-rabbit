namespace PLCLib;

public static class RabbitSettings
{
    public const string HostName = "localhost";

    public const int Port = 5672;

    public const string ExchangeName = "plc.exchange";

    public const string QueueName = "plc.queue.product";

    public const string RoutingKey = "plc.queue.*";
}
namespace PlcRabbitLibrary.Configuration;

public class RabbitQosConfig
{
    public uint PrefetchSize { get; set; } = 0;
    public ushort PrefetchCount { get; set; } = 1;
    public bool Global { get; set; } = false;
}

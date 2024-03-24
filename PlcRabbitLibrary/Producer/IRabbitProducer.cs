namespace PlcRabbitLibrary.Producer;

public interface IRabbitProducer<T>
{
    Task ProduceAsync(string exchange, string routingKey, T data);
}

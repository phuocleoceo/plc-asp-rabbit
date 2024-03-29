namespace PlcRabbitLibrary.Consumer;

public interface IRabbitConsumerHandler<T>
{
    string QueueName { get; }

    Task HandleAsync(T result);
}

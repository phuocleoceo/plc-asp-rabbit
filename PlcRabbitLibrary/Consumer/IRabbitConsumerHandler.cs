namespace PlcRabbitLibrary.Consumer;

public interface IRabbitConsumerHandler<T>
{
    Task HandleAsync(T result);
}

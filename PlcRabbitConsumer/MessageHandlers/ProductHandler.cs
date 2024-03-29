using PlcRabbitConsumer.Models;
using PlcRabbitLibrary.Consumer;

namespace PlcRabbitConsumer.MessageHandlers;

public class ProductHandler : IRabbitConsumerHandler<Product>
{
    public string QueueName => "plc.queue.product";

    public Task HandleAsync(Product product)
    {
        Console.WriteLine("Product: " + product);
        return Task.CompletedTask;
    }
}

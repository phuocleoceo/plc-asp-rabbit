using PlcRabbitConsumer.Models;
using PlcRabbitLibrary.Consumer;

namespace PlcRabbitConsumer.MessageHandlers;

public class UserHandler : IRabbitConsumerHandler<User>
{
    public string QueueName => "plc.queue.user";

    public Task HandleAsync(User user)
    {
        Console.WriteLine("User: " + user);
        return Task.CompletedTask;
    }
}

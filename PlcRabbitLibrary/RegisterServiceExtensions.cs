using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Consumer;
using PlcRabbitLibrary.Producer;

namespace PlcRabbitLibrary;

public static class RegisterServiceExtensions
{
    public static IServiceCollection AddRabbitProducer(
        this IServiceCollection services,
        Action<RabbitProducerConfig> configAction
    )
    {
        services.Configure(configAction);
        services.AddSingleton(typeof(IRabbitProducer<>), typeof(RabbitProducer<>));
        return services;
    }

    public static IServiceCollection AddRabbitConsumer<TValue, THandler>(
        this IServiceCollection services,
        Action<RabbitConsumerConfig> configAction
    )
        where THandler : class, IRabbitConsumerHandler<TValue>
    {
        services.Configure(configAction);
        services.AddScoped<IRabbitConsumerHandler<TValue>, THandler>();
        services.AddSingleton<IHostedService, RabbitConsumer<TValue>>();
        return services;
    }
}

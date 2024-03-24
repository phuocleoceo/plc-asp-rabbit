using Microsoft.Extensions.DependencyInjection;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Producer;

namespace PlcRabbitLibrary;

public static class RegisterServiceExtensions
{
    public static IServiceCollection AddKafkaProducer<T>(
        this IServiceCollection services,
        Action<RabbitProducerConfig> configAction
    )
    {
        services.Configure(configAction);
        services.AddSingleton(typeof(IRabbitProducer<>), typeof(RabbitProducer<>));
        return services;
    }
}

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PlcRabbitLibrary.Configuration;
using PlcRabbitLibrary.Consumer;
using PlcRabbitLibrary.Extensions;
using PlcRabbitLibrary.Producer;

namespace PlcRabbitLibrary;

public static class RegisterServiceExtensions
{
    private const string ConfigurationKey = "RabbitMQ";

    public static IServiceCollection AddRabbitConnection(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        services.Configure<RabbitMQConfig>(configuration.GetSection(ConfigurationKey));
        services.ConfigureRabbitConnection();
        return services;
    }

    public static IServiceCollection AddRabbitProducer(this IServiceCollection services)
    {
        services.AddSingleton(typeof(IRabbitProducer<>), typeof(RabbitProducer<>));
        return services;
    }

    public static IServiceCollection AddRabbitConsumer<TValue, THandler>(
        this IServiceCollection services
    )
        where THandler : class, IRabbitConsumerHandler<TValue>
    {
        services.AddScoped<IRabbitConsumerHandler<TValue>, THandler>();
        services.AddSingleton<IHostedService, RabbitConsumer<TValue>>();
        return services;
    }
}

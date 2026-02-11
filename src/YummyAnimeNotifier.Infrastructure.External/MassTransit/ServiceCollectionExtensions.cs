using YummyAnimeNotifier.Application.Events;
using YummyAnimeNotifier.Infrastructure.External.MassTransit.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YummyAnimeNotifier.Infrastructure.External.MassTransit;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<NewEpisodeDetected_UpdateAnimeConsumer>();

            x.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(configuration.GetConnectionString("RabbitMQ"));

                configurator.ConfigureEndpoints(context);
            });
        });
        services.AddScoped<IEventBus, MassTransitEventBus>();

        return services;
    }
}
using YummyAnimeNotifier.Application.Events;
using YummyAnimeNotifier.Infrastructure.External.MassTransit.Consumers;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YummyAnimeNotifier.Infrastructure.External.MassTransit;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMassTransit(
        this IServiceCollection services,
        IConfiguration configuration,
        bool registerConsumers)
    {
        services.AddMassTransit(x =>
        {
            if (registerConsumers)
            {
                x.AddConsumersFromNamespaceContaining<ReleaseCreated_UpdateAnimeTranslationConsumer>();
            }

            x.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(configuration.GetConnectionString("RabbitMQ"));

                if (registerConsumers)
                {
                    configurator.ConfigureEndpoints(context);
                }
            });
        });
        services.AddScoped<IEventBus, MassTransitEventBus>();

        return services;
    }
}
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YummyAnimeNotifier.Application.Worker.BackgroundServices;

namespace YummyAnimeNotifier.Application.Worker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWorkerApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication(configuration);

        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(config => { config.RegisterServicesFromAssemblies(assembly); });
        services.AddValidatorsFromAssembly(assembly);

        // services.AddHostedService<EpisodeTrackingBackgroundService>();
        services.AddHostedService<OutboxPublisherBackgroundService>();

        return services;
    }
}
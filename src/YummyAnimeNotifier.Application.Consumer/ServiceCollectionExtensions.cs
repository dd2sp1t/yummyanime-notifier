using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YummyAnimeNotifier.Application.Consumer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConsumerApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication(configuration);

        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(config => { config.RegisterServicesFromAssemblies(assembly); });
        services.AddValidatorsFromAssembly(assembly);

        return services;
    }
}
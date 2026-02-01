using AniMediaNotifier.Application.AniMedia;
using AniMediaNotifier.Application.AniMedia.Mappers;
using AniMediaNotifier.Application.Notifications;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AniMediaNotifier.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var siteDataSection = configuration.GetSection(nameof(AniMediaSiteData));
        services.Configure<AniMediaSiteData>(siteDataSection);

        services.AddSingleton<AnimeMapper>();

        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(config => { config.RegisterServicesFromAssemblies(assembly); });

        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        var notificationSettingsSection = configuration.GetSection(nameof(NotificationSettings));
        services.Configure<NotificationSettings>(notificationSettingsSection);

        return services;
    }
}
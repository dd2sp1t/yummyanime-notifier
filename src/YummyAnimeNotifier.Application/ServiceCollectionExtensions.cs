using YummyAnimeNotifier.Application.YummyAnime.Mappers;
using YummyAnimeNotifier.Application.Notifications;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using YummyAnimeNotifier.Application.YummyAnime;

namespace YummyAnimeNotifier.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var siteDataSection = configuration.GetSection(nameof(YummyAnimeSiteData));
        services.Configure<YummyAnimeSiteData>(siteDataSection);

        services.AddSingleton<AnimeMapper>();
        services.AddSingleton<AnimeTranslationDescriptorMapper>();
        services.AddSingleton<AnimeUpdateDescriptorMapper>();

        var assembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(config => { config.RegisterServicesFromAssemblies(assembly); });

        services.AddValidatorsFromAssembly(assembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        var notificationSettingsSection = configuration.GetSection(nameof(NotificationSettings));
        services.Configure<NotificationSettings>(notificationSettingsSection);

        return services;
    }
}
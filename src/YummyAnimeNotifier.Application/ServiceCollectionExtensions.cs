using YummyAnimeNotifier.Application.YummyAnime.Mappers;
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
        services.AddSingleton<AnimeTranslationUpdateDescriptorMapper>();

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }
}
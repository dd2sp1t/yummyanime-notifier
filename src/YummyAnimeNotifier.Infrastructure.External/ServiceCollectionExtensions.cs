using YummyAnimeNotifier.Infrastructure.External.YummyAnime;
using YummyAnimeNotifier.Infrastructure.External.MassTransit;
using YummyAnimeNotifier.Infrastructure.External.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YummyAnimeNotifier.Infrastructure.External;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddYummyAnimeTools(configuration)
            .AddMassTransit(configuration)
            .AddTelegram(configuration);
    }
}
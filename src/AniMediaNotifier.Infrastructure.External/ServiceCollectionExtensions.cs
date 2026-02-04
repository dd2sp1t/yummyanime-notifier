using AniMediaNotifier.Infrastructure.External.AniMedia;
using AniMediaNotifier.Infrastructure.External.MassTransit;
using AniMediaNotifier.Infrastructure.External.Telegram;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AniMediaNotifier.Infrastructure.External;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddAniMediaTools(configuration)
            .AddMassTransit(configuration)
            .AddTelegram(configuration);
    }
}
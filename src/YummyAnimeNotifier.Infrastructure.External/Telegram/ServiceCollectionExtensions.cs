using YummyAnimeNotifier.Application.Consumer.Notifications.Formatters;
using YummyAnimeNotifier.Application.Consumer.Notifications.Senders;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using YummyAnimeNotifier.Infrastructure.External.Telegram.Sending.NotificationFormatters;
using YummyAnimeNotifier.Infrastructure.External.Telegram.Sending;
using YummyAnimeNotifier.Infrastructure.External.Telegram.Receiving.Updates.Handlers;
using YummyAnimeNotifier.Infrastructure.External.Telegram.Receiving.Updates;
using YummyAnimeNotifier.Infrastructure.External.Telegram.Receiving;

namespace YummyAnimeNotifier.Infrastructure.External.Telegram;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegramSending(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTelegramBase(configuration);

        services.AddScoped<INotificationFormatter, TelegramHtmlNotificationFormatter>();
        // services.AddScoped<INotificationFormatter, TelegramMarkdownV2NotificationFormatter>();
        services.AddScoped<INotificationSender, TelegramNotificationSender>();

        return services;
    }

    public static IServiceCollection AddTelegramReceiving(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTelegramBase(configuration);

        services.AddHostedService<TelegramBotBackgroundService>();

        services.AddSingleton<ITelegramUpdateRouter, TelegramUpdateRouter>();
        services.AddScoped<ITelegramUpdateHandler, SubscribeCommandHandler>();

        return services;
    }

    private static IServiceCollection AddTelegramBase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var botSettingsSection = configuration.GetSection(nameof(TelegramBotSettings));
        services.Configure<TelegramBotSettings>(botSettingsSection);

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var botSettings = sp.GetRequiredService<IOptions<TelegramBotSettings>>().Value;
            return new TelegramBotClient(botSettings.Token);
        });

        return services;
    }
}
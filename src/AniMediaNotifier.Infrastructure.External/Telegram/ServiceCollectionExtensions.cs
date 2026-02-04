using AniMediaNotifier.Application.Notifications.Formatters;
using AniMediaNotifier.Application.Notifications.Senders;
using AniMediaNotifier.Infrastructure.External.Telegram.BackgroundServices;
using AniMediaNotifier.Infrastructure.External.Telegram.Notifications;
using AniMediaNotifier.Infrastructure.External.Telegram.Notifications.Formatters;
using AniMediaNotifier.Infrastructure.External.Telegram.Updates;
using AniMediaNotifier.Infrastructure.External.Telegram.Updates.Handlers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace AniMediaNotifier.Infrastructure.External.Telegram;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTelegram(
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

        services.AddScoped<INotificationFormatter, TelegramHtmlNotificationFormatter>();
        // services.AddScoped<INotificationFormatter, TelegramMarkdownV2NotificationFormatter>();
        services.AddScoped<INotificationSender, TelegramNotificationSender>();

        services.AddHostedService<TelegramBotBackgroundService>();
        services.AddSingleton<ITelegramUpdateRouter, TelegramUpdateRouter>();

        services.AddScoped<ITelegramUpdateHandler, SubscribeCommandHandler>();

        return services;
    }
}
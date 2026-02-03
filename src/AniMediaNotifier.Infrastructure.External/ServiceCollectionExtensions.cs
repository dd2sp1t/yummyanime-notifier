using AniMediaNotifier.Application.AniMedia.Client;
using AniMediaNotifier.Application.AniMedia.Parsers;
using AniMediaNotifier.Application.Events;
using AniMediaNotifier.Application.Notifications.Senders;
using AniMediaNotifier.Infrastructure.External.AniMedia.Client;
using AniMediaNotifier.Infrastructure.External.AniMedia.Parsers;
using AniMediaNotifier.Infrastructure.External.Events.MassTransit;
using AniMediaNotifier.Infrastructure.External.Events.MassTransit.Consumers;
using AniMediaNotifier.Infrastructure.External.Notifications.Formatters;
using AniMediaNotifier.Infrastructure.External.Notifications.Senders;
using AniMediaNotifier.Infrastructure.External.Notifications.Senders.Telegram;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;
using Telegram.Bot;

namespace AniMediaNotifier.Infrastructure.External;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddEvents(configuration)
            .AddAniMediaTools(configuration)
            .AddNotificationSender(configuration);
    }

    private static IServiceCollection AddEvents(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumersFromNamespaceContaining<NewEpisodeDetected_UpdateAnimeConsumer>();

            x.UsingRabbitMq((context, configurator) =>
            {
                configurator.Host(configuration.GetConnectionString("RabbitMQ"));

                configurator.ConfigureEndpoints(context);
            });
        });
        services.AddScoped<IEventBus, MassTransitEventBus>();

        return services;
    }

    private static IServiceCollection AddAniMediaTools(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddAniMediaClient(configuration)
            .AddAniMediaParsers();
    }

    private static IServiceCollection AddAniMediaClient(this IServiceCollection services, IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection(nameof(AniMediaClientSettings));
        services.Configure<AniMediaClientSettings>(settingsSection);

        var random = new Random();

        services
            .AddHttpClient<IAniMediaClient, AniMediaClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<AniMediaClientSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);

            })
            .AddPolicyHandler((sp, request) =>
            {
                var settings = sp.GetRequiredService<IOptions<AniMediaClientSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<AniMediaClient>>();
                var random = new Random();

                return Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .Or<TimeoutRejectedException>()
                    .WaitAndRetryAsync(
                        retryCount: settings.MaxRetryCount,
                        sleepDurationProvider: retryAttempt =>
                        {
                            var timespan = TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))
                                + TimeSpan.FromMilliseconds(random.Next(0, 1000));

                            return timespan;
                        },
                        onRetry: (outcome, timespan, retryAttempt, context) =>
                        {
                            logger.Log(
                                LogLevel.Warning,
                                "{ClassName}: Retrying... attempt {Attempt}, waiting {Delay}s",
                                nameof(AniMediaClient),
                                retryAttempt,
                                timespan.TotalSeconds);
                        }
                    );
            })
            .AddPolicyHandler((sp, request) =>
            {
                var settings = sp.GetRequiredService<IOptions<AniMediaClientSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<AniMediaClient>>();

                return Policy<HttpResponseMessage>
                    .Handle<HttpRequestException>()
                    .Or<TimeoutRejectedException>()
                    .CircuitBreakerAsync(
                        handledEventsAllowedBeforeBreaking: settings.CircuitBreakerEventCount,
                        durationOfBreak: TimeSpan.FromSeconds(settings.CircuitBreakerDurationSeconds),
                        onBreak: (outcome, timespan) =>
                        {
                            logger.Log(
                                LogLevel.Warning,
                                "{ClassName}: Circuit open for {Delay}s",
                                nameof(AniMediaClient),
                                timespan.TotalSeconds);
                        },
                        onReset: () =>
                        {
                            logger.LogInformation("{ClassName}: Circuit closed", nameof(AniMediaClient));
                        }
                    );
            });

        return services;
    }

    private static IServiceCollection AddAniMediaParsers(this IServiceCollection services)
    {
        services.AddSingleton<IAnimePageParser, AnimePageParser>();
        services.AddSingleton<IEpisodeWidgetParser, EpisodeWidgetParser>();

        return services;
    }

    private static IServiceCollection AddNotificationSender(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var botSettingsSection = configuration.GetSection(nameof(TelegramBotSettings));
        services.Configure<TelegramBotSettings>(botSettingsSection);

        services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<TelegramBotSettings>>().Value;
            return new TelegramBotClient(settings.Token);
        });

        services.AddScoped<INotificationFormatter, HtmlNotificationFormatter>();
        // services.AddScoped<INotificationFormatter, MarkdownNotificationFormatter>();

        // services.AddScoped<INotificationSender, MockNotificationSender>();
        services.AddScoped<INotificationSender, TelegramNotificationSender>();

        return services;
    }
}
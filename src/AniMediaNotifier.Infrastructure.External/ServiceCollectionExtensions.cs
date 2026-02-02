using AniMediaNotifier.Application.AniMedia.Client;
using AniMediaNotifier.Application.AniMedia.Parsers;
using AniMediaNotifier.Application.Events;
using AniMediaNotifier.Application.Notifications;
using AniMediaNotifier.Infrastructure.External.AniMedia.Client;
using AniMediaNotifier.Infrastructure.External.AniMedia.Parsers;
using AniMediaNotifier.Infrastructure.External.Events.MassTransit;
using AniMediaNotifier.Infrastructure.External.Events.MassTransit.Consumers;
using AniMediaNotifier.Infrastructure.External.NotificationSenders;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;

namespace AniMediaNotifier.Infrastructure.External;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExternal(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        return services
            .AddAniMediaClient(configuration)
            .AddAniMediaParsers()
            .AddNotificationSender()
            .AddEvents(configuration);
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
                            logger.LogWarning(
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
                            logger.LogWarning(
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

    private static IServiceCollection AddNotificationSender(this IServiceCollection services)
    {
        services.AddScoped<INotificationSender, MockNotificationSender>();

        return services;
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
}
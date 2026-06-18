using YummyAnimeNotifier.Application.YummyAnime.Client;
using YummyAnimeNotifier.Application.YummyAnime.Parsers;
using YummyAnimeNotifier.Infrastructure.External.YummyAnime.Client;
using YummyAnimeNotifier.Infrastructure.External.YummyAnime.Parsers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Timeout;

namespace YummyAnimeNotifier.Infrastructure.External.YummyAnime;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddYummyAnimeTools(this IServiceCollection services, IConfiguration configuration)
    {
        return services
            .AddYummyAnimeClient(configuration)
            .AddYummyAnimeParsers();
    }

    private static IServiceCollection AddYummyAnimeClient(this IServiceCollection services, IConfiguration configuration)
    {
        var settingsSection = configuration.GetSection(nameof(YummyAnimeClientSettings));
        services.Configure<YummyAnimeClientSettings>(settingsSection);

        var random = new Random();

        services
            .AddHttpClient<IYummyAnimeClient, YummyAnimeClient>((sp, client) =>
            {
                var settings = sp.GetRequiredService<IOptions<YummyAnimeClientSettings>>().Value;
                client.BaseAddress = new Uri(settings.BaseAddress);
                client.Timeout = TimeSpan.FromSeconds(settings.TimeoutSeconds);
                client.DefaultRequestHeaders.Add("X-Application", settings.Token);
            })
            .AddPolicyHandler((sp, request) =>
            {
                var settings = sp.GetRequiredService<IOptions<YummyAnimeClientSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<YummyAnimeClient>>();
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
                                nameof(YummyAnimeClient),
                                retryAttempt,
                                timespan.TotalSeconds);
                        }
                    );
            })
            .AddPolicyHandler((sp, request) =>
            {
                var settings = sp.GetRequiredService<IOptions<YummyAnimeClientSettings>>().Value;
                var logger = sp.GetRequiredService<ILogger<YummyAnimeClient>>();

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
                                nameof(YummyAnimeClient),
                                timespan.TotalSeconds);
                        },
                        onReset: () =>
                        {
                            logger.LogInformation("{ClassName}: Circuit closed", nameof(YummyAnimeClient));
                        }
                    );
            });

        return services;
    }

    private static IServiceCollection AddYummyAnimeParsers(this IServiceCollection services)
    {
        services.AddSingleton<IAnimeParser, AnimeParser>();
        services.AddSingleton<IAnimeUpdateParser, AnimeUpdateParser>();
        services.AddSingleton<IAnimeTranslationParser, AnimeTranslationParser>();

        return services;
    }
}
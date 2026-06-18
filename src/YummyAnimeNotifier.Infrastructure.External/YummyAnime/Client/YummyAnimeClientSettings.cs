namespace YummyAnimeNotifier.Infrastructure.External.YummyAnime.Client;

public class YummyAnimeClientSettings
{
    public string BaseAddress { get; init; }
    public string Token { get; init; }
    public int TimeoutSeconds { get; init; }
    public int MaxRetryCount { get; init; }
    public int CircuitBreakerEventCount { get; init; }
    public int CircuitBreakerDurationSeconds { get; init; }
}
namespace AniMediaNotifier.Infrastructure.External.AniMedia.Client;

public class AniMediaClientSettings
{
    public string BaseAddress { get; init; }
    public int TimeoutSeconds { get; init; }
    public int MaxRetryCount { get; init; }
    public int CircuitBreakerEventCount { get; init; }
    public int CircuitBreakerDurationSeconds { get; init; }
}
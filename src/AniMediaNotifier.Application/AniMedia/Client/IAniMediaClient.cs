namespace AniMediaNotifier.Application.AniMedia.Client;

public interface IAniMediaClient
{
    Task<string> GetHtmlStringAsync(Uri uri, CancellationToken cancellationToken = default);
}
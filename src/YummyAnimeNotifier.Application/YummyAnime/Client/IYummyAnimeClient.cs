namespace YummyAnimeNotifier.Application.YummyAnime.Client;

public interface IYummyAnimeClient
{
    Task<string> GetHtmlStringAsync(Uri uri, CancellationToken cancellationToken = default);
    Task<string> GetVideosJsonAsync(int externalId, CancellationToken cancellationToken = default);
}
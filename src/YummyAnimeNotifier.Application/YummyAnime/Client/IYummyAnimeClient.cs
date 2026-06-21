namespace YummyAnimeNotifier.Application.YummyAnime.Client;

public interface IYummyAnimeClient
{
    Task<HttpFetchResult> GetHtmlStringAsync(Uri uri, CancellationToken cancellationToken = default);
    Task<HttpFetchResult> GetVideosJsonAsync(int externalId, CancellationToken cancellationToken = default);
}
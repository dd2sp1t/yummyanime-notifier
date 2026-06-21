using YummyAnimeNotifier.Application.YummyAnime.Client;
using YummyAnimeNotifier.Application.YummyAnime.Exceptions;

namespace YummyAnimeNotifier.Infrastructure.External.YummyAnime.Client;

internal class YummyAnimeClient : IYummyAnimeClient
{
    private readonly HttpClient _httpClient;

    public YummyAnimeClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<HttpFetchResult> GetHtmlStringAsync(Uri uri, CancellationToken cancellationToken)
    {
        var uriBelongToClientDomain = Uri.Compare(
            _httpClient.BaseAddress,
            uri,
            UriComponents.SchemeAndServer,
            UriFormat.Unescaped,
            StringComparison.OrdinalIgnoreCase) == 0;

        if (uriBelongToClientDomain == false)
        {
            throw new ClientException($"Uri '{uri}' does not belong to domain {_httpClient.BaseAddress}");
        }

        var response = await _httpClient.GetAsync(uri.PathAndQuery, cancellationToken);

        var content = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken);

        return new HttpFetchResult(
            response.IsSuccessStatusCode,
            (int)response.StatusCode,
            content);
    }

    public async Task<HttpFetchResult> GetVideosJsonAsync(int externalId, CancellationToken cancellationToken)
    {
        var uri = new Uri(_httpClient.BaseAddress, $"/api/anime/{externalId}/videos");

        var response = await _httpClient.GetAsync(uri, cancellationToken);

        var content = response.Content is null
            ? null
            : await response.Content.ReadAsStringAsync(cancellationToken);

        return new HttpFetchResult(
            response.IsSuccessStatusCode,
            (int)response.StatusCode,
            content);
    }
}
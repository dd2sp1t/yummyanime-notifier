using AniMediaNotifier.Application.AniMedia.Client;
using AniMediaNotifier.Application.AniMedia.Exceptions;

namespace AniMediaNotifier.Infrastructure.External.AniMedia.Client;

internal class AniMediaClient : IAniMediaClient
{
    private readonly HttpClient _httpClient;

    public AniMediaClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetHtmlStringAsync(Uri uri, CancellationToken cancellationToken)
    {
        var uriBelongToClientDomain = Uri.Compare(
            _httpClient.BaseAddress,
            uri,
            UriComponents.SchemeAndServer,
            UriFormat.Unescaped,
            StringComparison.OrdinalIgnoreCase) == 0;

        if (uriBelongToClientDomain == false)
        {
            throw new AniMediaClientException($"Uri '{uri}' does not belong to domain {_httpClient.BaseAddress}");
        }

        var response = await _httpClient.GetAsync(uri.PathAndQuery, cancellationToken);

        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return content;
    }
}
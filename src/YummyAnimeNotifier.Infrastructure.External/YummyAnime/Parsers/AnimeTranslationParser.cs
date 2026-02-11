using System.Text.Json;
using System.Text.Json.Serialization;
using YummyAnimeNotifier.Application.YummyAnime.Parsers;
using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Infrastructure.External.YummyAnime.Parsers;

internal class AnimeTranslationParser : IAnimeTranslationParser
{
    public ParsedAnimeTranslation[] Parse(string json)
    {
        if (string.IsNullOrWhiteSpace(json))
        {
            return [];
        }

        var root = JsonSerializer.Deserialize<RootJson>(json);
        if (root?.Response is null || root.Response.Count == 0)
        {
            return [];
        }

        var parsed = root.Response
            .Select(v => new
            {
                v.Data.Dubbing,
                v.Number
            })
            .GroupBy(v => v.Dubbing)
            .Select(v => new ParsedAnimeTranslation(
                TranslationRaw: v.Key,
                MaxEpisodeNumber: v.Max(v => int.Parse(v.Number))))
            .ToArray();

        return parsed;
    }

    #region JSON DTOs

    private class RootJson
    {
        [JsonPropertyName("response")]
        public List<VideoJson> Response { get; set; }
    }

    private class VideoJson
    {
        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("data")]
        public VideoDataJson Data { get; set; }
    }

    private class VideoDataJson
    {
        [JsonPropertyName("dubbing")]
        public string Dubbing { get; set; }
    }

    #endregion
}
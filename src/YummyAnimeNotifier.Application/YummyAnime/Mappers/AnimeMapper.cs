using YummyAnimeNotifier.Application.YummyAnime.Exceptions;
using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;
using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.YummyAnime.Mappers;

public class AnimeMapper
{
    public Domain.Entities.Anime Map(ParsedAnime parsed)
    {
        return Domain.Entities.Anime.Create(
            parsed.ExternalId,
            parsed.SourceLink,
            parsed.Name,
            MapType(parsed.TypeRaw),
            MapStatus(parsed.StatusRaw),
            parsed.ReleasedEpisodes,
            parsed.TotalEpisodes);
    }

    private static AnimeType MapType(string raw)
    {
        return raw.ToLower() switch
        {
            "сериал" => AnimeType.Serial,
            "малометражный сериал" => AnimeType.Serial,
            "ona" => AnimeType.Ona,
            "ova" => AnimeType.Ova,
            "полнометражный фильм" => AnimeType.Movie,
            _ => throw new MappingException($"Unknown anime type: '{raw.ToLower()}'")
        };
    }

    private static AnimeStatus MapStatus(string raw)
    {
        return raw.ToLower() switch
        {
            "анонс" => AnimeStatus.Preview,
            "онгоинг" => AnimeStatus.Ongoing,
            "вышел" => AnimeStatus.Finished,
            _ => throw new MappingException($"Unknown anime status: '{raw.ToLower()}'")
        };
    }
}
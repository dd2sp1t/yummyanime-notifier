using AniMediaNotifier.Application.AniMedia.Exceptions;
using AniMediaNotifier.Application.AniMedia.Parsers.Models;
using AniMediaNotifier.Domain.Enums;

namespace AniMediaNotifier.Application.AniMedia.Mappers;

public class AnimeMapper
{
    public Domain.Entities.Anime Map(ParsedAnimeInfo parsed)
    {
        return Domain.Entities.Anime.Create(
            parsed.SourceLink,
            parsed.OriginalName,
            parsed.RuName,
            parsed.Year,
            MapType(parsed.TypeRaw),
            MapStatus(parsed.StatusRaw),
            parsed.ReleasedEpisodeCount,
            parsed.TotalEpisodeCount);
    }

    private static AnimeType MapType(string raw)
    {
        return raw switch
        {
            "ТВ-сериал" => AnimeType.TvSerial,
            "ONA" => AnimeType.Ona,
            "OVA" => AnimeType.Ova,
            "Фильм" => AnimeType.Movie,
            "Компиляция" => AnimeType.Compilation,
            _ => throw new AnimeMappingException($"Unknown anime type: '{raw}'")
        };
    }

    private static AnimeStatus MapStatus(string raw)
    {
        return raw switch
        {
            "Анонсы" => AnimeStatus.Preview,
            "Онгоинги" => AnimeStatus.Ongoing,
            "Завершенные" => AnimeStatus.Finished,
            _ => throw new AnimeMappingException($"Unknown anime status: '{raw}'")
        };
    }
}
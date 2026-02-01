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

    private static AnimeTypeEnum MapType(string raw)
    {
        return raw switch
        {
            "ТВ-сериал" => AnimeTypeEnum.TvSerial,
            "ONA" => AnimeTypeEnum.Ona,
            "OVA" => AnimeTypeEnum.Ova,
            "Фильм" => AnimeTypeEnum.Movie,
            "Компиляция" => AnimeTypeEnum.Compilation,
            _ => throw new AnimeMappingException($"Unknown anime type: '{raw}'")
        };
    }

    private static AnimeStatusEnum MapStatus(string raw)
    {
        return raw switch
        {
            "Анонсы" => AnimeStatusEnum.Preview,
            "Онгоинги" => AnimeStatusEnum.Ongoing,
            "Завершенные" => AnimeStatusEnum.Finished,
            _ => throw new AnimeMappingException($"Unknown anime status: '{raw}'")
        };
    }
}
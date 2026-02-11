using YummyAnimeNotifier.Application.YummyAnime.Mappers.Helpers;
using YummyAnimeNotifier.Application.YummyAnime.Mappers.Models;
using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Application.YummyAnime.Mappers;

public class AnimeUpdateDescriptorMapper
{
    public AnimeUpdateDescriptor Map(ParsedAnimeUpdate parsed)
    {
        var (translationType, translationSourceName) = TranslationDataExtractor.Extract(parsed.TranslationRaw);

        return new AnimeUpdateDescriptor(
            parsed.AnimeName,
            parsed.EpisodeNumber,
            translationType,
            translationSourceName);
    }
}
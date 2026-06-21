using YummyAnimeNotifier.Application.YummyAnime.Mappers.Helpers;
using YummyAnimeNotifier.Application.YummyAnime.Mappers.Models;
using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Application.YummyAnime.Mappers;

public class AnimeTranslationUpdateDescriptorMapper
{
    public AnimeTranslationUpdateDescriptor Map(ParsedAnimeTranslationUpdate parsed)
    {
        var (translationType, translationSourceName) = TranslationDataExtractor.Extract(parsed.TranslationRaw);

        return new AnimeTranslationUpdateDescriptor(
            parsed.AnimeName,
            translationType,
            translationSourceName,
            parsed.EpisodeNumber);
    }
}
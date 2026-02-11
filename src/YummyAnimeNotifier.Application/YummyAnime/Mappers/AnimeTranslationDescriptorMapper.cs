using YummyAnimeNotifier.Application.YummyAnime.Mappers.Helpers;
using YummyAnimeNotifier.Application.YummyAnime.Mappers.Models;
using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Application.YummyAnime.Mappers;

public class AnimeTranslationDescriptorMapper
{
    public AnimeTranslationDescriptor Map(ParsedAnimeTranslation parsed)
    {
        var (translationType, translationSourceName) = TranslationDataExtractor.Extract(parsed.TranslationRaw);

        return new AnimeTranslationDescriptor(translationType, translationSourceName, parsed.MaxEpisodeNumber);
    }
}
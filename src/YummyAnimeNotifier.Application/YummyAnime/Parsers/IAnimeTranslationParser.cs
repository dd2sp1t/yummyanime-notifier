using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Application.YummyAnime.Parsers;

public interface IAnimeTranslationParser
{
    ParsedAnimeTranslation[] Parse(string json);
}
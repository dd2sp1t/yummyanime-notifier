using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Application.YummyAnime.Parsers;

public interface IAnimeTranslationUpdateParser
{
    ParsedAnimeTranslationUpdate[] Parse(string html);
}
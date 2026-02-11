using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Application.YummyAnime.Parsers;

public interface IAnimeUpdateParser
{
    ParsedAnimeUpdate[] Parse(string html);
}
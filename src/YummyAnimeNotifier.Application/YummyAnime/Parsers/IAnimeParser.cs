using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;

namespace YummyAnimeNotifier.Application.YummyAnime.Parsers;

public interface IAnimeParser
{
    ParsedAnime Parse(string html);
}
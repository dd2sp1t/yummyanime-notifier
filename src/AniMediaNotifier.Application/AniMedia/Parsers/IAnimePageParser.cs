using AniMediaNotifier.Application.AniMedia.Parsers.Models;

namespace AniMediaNotifier.Application.AniMedia.Parsers;

public interface IAnimePageParser
{
    ParsedAnimeInfo Parse(string html);
}
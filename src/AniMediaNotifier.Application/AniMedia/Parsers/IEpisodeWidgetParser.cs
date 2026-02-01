using AniMediaNotifier.Application.AniMedia.Parsers.Models;

namespace AniMediaNotifier.Application.AniMedia.Parsers;

public interface IEpisodeWidgetParser
{
    ParsedEpisodeInfo[] Parse(string html);
}
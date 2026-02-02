using AniMediaNotifier.Application.AniMedia;
using AniMediaNotifier.Application.AniMedia.Client;
using AniMediaNotifier.Application.AniMedia.Parsers;
using AniMediaNotifier.Application.AniMedia.Parsers.Models;
using AniMediaNotifier.Application.Events;
using AniMediaNotifier.Application.Repositories;
using AniMediaNotifier.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;

namespace AniMediaNotifier.Application.Notifications.Commands.CheckNewEpisodes;

public class CheckNewEpisodesHandler : IRequestHandler<CheckNewEpisodesCommand, Unit>
{
    private readonly AniMediaSiteData _aniMediaSiteData;
    private readonly IAniMediaClient _aniMediaClient;
    private readonly IEpisodeWidgetParser _episodeWidgetParser;
    private readonly IAnimeRepository _animeRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;

    public CheckNewEpisodesHandler(
        IOptions<AniMediaSiteData> options,
        IAniMediaClient aniMediaClient,
        IEpisodeWidgetParser episodeWidgetParser,
        IAnimeRepository animeRepository,
        IOutboxMessageRepository outboxMessageRepository)
    {
        _aniMediaSiteData = options.Value;
        _aniMediaClient = aniMediaClient;
        _episodeWidgetParser = episodeWidgetParser;
        _animeRepository = animeRepository;
        _outboxMessageRepository = outboxMessageRepository;
    }

    public async Task<Unit> Handle(CheckNewEpisodesCommand request, CancellationToken cancellationToken)
    {
        var parsed = await ParseEpisodesAsync(cancellationToken);

        var animes = await GetAnimesAsync(parsed, cancellationToken);

        var joined = parsed.Join(
            animes,
            p => p.RuName,
            a => a.RuName,
            (p, a) => new
            {
                p.RuName,
                p.EpisodeNumber,
                AnimeId = a.Id,
                a.ReleasedEpisodeCount,
                a.TotalEpisodeCount
            });

        var outboxMessages = joined
            .Where(j => j.EpisodeNumber > j.ReleasedEpisodeCount)
            .Select(j =>
            {
                var @event = new NewEpisodeDetectedEvent(j.AnimeId, j.EpisodeNumber);

                return OutboxMessage.Create(@event);
            })
            .ToArray();

        await _outboxMessageRepository.AddRangeAsync(outboxMessages, cancellationToken);

        return Unit.Value;
    }

    private async Task<ParsedEpisodeInfo[]> ParseEpisodesAsync(CancellationToken cancellationToken)
    {
        var uri = new Uri($"{_aniMediaSiteData.Domain}{_aniMediaSiteData.EpisodeWidgetRelativeLink}");

        var html = await _aniMediaClient.GetHtmlStringAsync(uri, cancellationToken);

        var parsed = _episodeWidgetParser.Parse(html);

        return parsed;
    }

    private async Task<Domain.Entities.Anime[]> GetAnimesAsync(
        ParsedEpisodeInfo[] parsed,
        CancellationToken cancellationToken)
    {
        var tasks = parsed
            .Select(p => p.RuName)
            .Distinct()
            .Select(ruName => _animeRepository.FindByRuNameAsync(ruName, cancellationToken))
            .ToArray();

        var animes = await Task.WhenAll(tasks);

        return [.. animes.Where(a => a is not null)];
    }
}
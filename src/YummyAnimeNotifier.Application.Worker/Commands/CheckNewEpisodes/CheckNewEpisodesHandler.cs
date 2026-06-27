using YummyAnimeNotifier.Application.YummyAnime.Client;
using YummyAnimeNotifier.Application.YummyAnime.Parsers;
using YummyAnimeNotifier.Application.Events;
using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using YummyAnimeNotifier.Application.YummyAnime.Parsers.Models;
using YummyAnimeNotifier.Application.YummyAnime;
using YummyAnimeNotifier.Application.YummyAnime.Mappers;
using YummyAnimeNotifier.Application.YummyAnime.Mappers.Models;
using YummyAnimeNotifier.Domain.Enums;
using YummyAnimeNotifier.Application.YummyAnime.Exceptions;

namespace YummyAnimeNotifier.Application.Worker.Commands.CheckNewEpisodes;

public class CheckNewEpisodesHandler : IRequestHandler<CheckNewEpisodesCommand, Unit>
{
    private readonly YummyAnimeSiteData _yummyAnimeSiteData;
    private readonly IYummyAnimeClient _yummyAnimeClient;
    private readonly IAnimeTranslationUpdateParser _animeTranslationUpdateParser;
    private readonly AnimeTranslationUpdateDescriptorMapper _animeTranslationUpdateDescriptorMapper;
    private readonly IAnimeRepository _animeRepository;
    private readonly IAnimeTranslationRepository _animeTranslationRepository;
    private readonly ITranslationSourceRepository _translationSourceRepository;
    private readonly IReleaseRepository _releaseRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckNewEpisodesHandler(
        IOptions<YummyAnimeSiteData> options,
        IYummyAnimeClient yummyAnimeClient,
        IAnimeTranslationUpdateParser animeTranslationUpdateParser,
        AnimeTranslationUpdateDescriptorMapper animeTranslationUpdateDescriptorMapper,
        IAnimeRepository animeRepository,
        IAnimeTranslationRepository animeTranslationRepository,
        ITranslationSourceRepository translationSourceRepository,
        IReleaseRepository releaseRepository,
        IOutboxMessageRepository outboxMessageRepository,
        IUnitOfWork unitOfWork)
    {
        _yummyAnimeSiteData = options.Value;
        _yummyAnimeClient = yummyAnimeClient;
        _animeTranslationUpdateParser = animeTranslationUpdateParser;
        _animeTranslationUpdateDescriptorMapper = animeTranslationUpdateDescriptorMapper;
        _animeRepository = animeRepository;
        _animeTranslationRepository = animeTranslationRepository;
        _translationSourceRepository = translationSourceRepository;
        _releaseRepository = releaseRepository;
        _outboxMessageRepository = outboxMessageRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(CheckNewEpisodesCommand request, CancellationToken cancellationToken)
    {
        var parsed = await ParseAnimeTranslationUpdatesAsync(cancellationToken);

        var updateDescriptors = parsed
            .Select(_animeTranslationUpdateDescriptorMapper.Map)
            .ToArray();

        var animeTranslations = await FindAnimeTranslationsAsync(updateDescriptors, cancellationToken);
        if (animeTranslations.Length == 0)
        {
            return Unit.Value;
        }

        var animeIds = animeTranslations
            .Select(at => at.AnimeId)
            .Distinct()
            .ToArray();
        var animes = await _animeRepository.GetAsync(animeIds, cancellationToken);

        var translationSourceIds = animeTranslations
            .Select(at => at.TranslationSourceId)
            .Distinct()
            .ToArray();
        var translationSources = await _translationSourceRepository.GetAsync(translationSourceIds, cancellationToken);

        var releases = GetReleases(animeTranslations, translationSources, animes, updateDescriptors);
        await SaveReleasesAsync(releases, cancellationToken);

        return Unit.Value;
    }

    private async Task<ParsedAnimeTranslationUpdate[]> ParseAnimeTranslationUpdatesAsync(
        CancellationToken cancellationToken)
    {
        var uriString = $"{_yummyAnimeSiteData.Domain}{_yummyAnimeSiteData.AnimeUpdatesRelativeLink}";

        var result = await _yummyAnimeClient.GetHtmlStringAsync(new Uri(uriString), cancellationToken);

        if (result.IsSuccess == false || string.IsNullOrWhiteSpace(result.Content))
        {
            throw new ClientException($"Failed to fetch anime updates page. Status: {result.StatusCode}");
        }

        var parsed = _animeTranslationUpdateParser.Parse(result.Content);

        return parsed;
    }

    private async Task<AnimeTranslation[]> FindAnimeTranslationsAsync(
        AnimeTranslationUpdateDescriptor[] updateDescriptors,
        CancellationToken cancellationToken)
    {
        var grouped = updateDescriptors.GroupBy(d => d.AnimeName);

        var animeNames = grouped.Select(g => g.Key).ToArray();
        var subscribedAnimeNames = await _animeRepository.FilterSubscribedNamesAsync(animeNames, cancellationToken);
        if (subscribedAnimeNames.Length == 0)
        {
            return [];
        }

        var subscribedAnimeNameSet = subscribedAnimeNames.ToHashSet();

        var result = new List<AnimeTranslation>();
        foreach (var group in grouped)
        {
            if (subscribedAnimeNameSet.Contains(group.Key) == false)
            {
                continue;
            }

            var translations = await FindAnimeTranslationsAsync(
                animeName: group.Key,
                updateDescriptors: [.. group],
                cancellationToken);

            result.AddRange(translations);
        }

        return [.. result];
    }

    private async Task<AnimeTranslation[]> FindAnimeTranslationsAsync(
        string animeName,
        AnimeTranslationUpdateDescriptor[] updateDescriptors,
        CancellationToken cancellationToken)
    {
        var grouped = updateDescriptors
            .GroupBy(
                keySelector: group => group.TranslationType,
                elementSelector: group => group.TranslationSourceName)
            .ToArray();

        var result = new List<AnimeTranslation>();
        foreach (var group in grouped)
        {
            var translationSourceNames = group.Distinct().ToArray();
            var translations = await _animeTranslationRepository.FindAsync(
                animeName,
                translationType: group.Key,
                translationSourceNames,
                cancellationToken);

            result.AddRange(translations);
        }

        return [.. result];
    }

    private Release[] GetReleases(
        AnimeTranslation[] animeTranslations,
        TranslationSource[] translationSources,
        Domain.Entities.Anime[] animes,
        AnimeTranslationUpdateDescriptor[] updateDescriptors)
    {
        var translationSourceMap = translationSources.ToDictionary(t => t.Id);
        var animeMap = animes.ToDictionary(a => a.Id);

        var episodeNumbersMap = updateDescriptors
            .GroupBy(
                d => new UpdateDescriptorKey(
                    d.AnimeName,
                    d.TranslationType,
                    d.TranslationSourceName))
            .ToDictionary(
                g => g.Key,
                g => g
                    .Select(g => g.EpisodeNumber)
                    .Distinct()
                    .OrderBy(n => n)
                    .ToArray());

        var result = new List<Release>();

        foreach (var translation in animeTranslations)
        {
            if (animeMap.TryGetValue(translation.AnimeId, out var anime) == false)
            {
                continue;
            }

            if (translationSourceMap.TryGetValue(translation.TranslationSourceId, out var translationSource) == false)
            {
                continue;
            }

            var key = new UpdateDescriptorKey(anime.Name, translationSource.Type, translationSource.Name);
            if (episodeNumbersMap.TryGetValue(key, out var episodeNumbers) == false)
            {
                continue;
            }

            var releases = episodeNumbers
                .Where(n => n > translation.ReleasedEpisodes)
                .Select(n => Release.Create(anime.Id, translationSource.Id, episodeNumber: n))
                .ToArray();

            result.AddRange(releases);
        }

        return [.. result];
    }

    private async Task SaveReleasesAsync(Release[] releases, CancellationToken cancellationToken)
    {
        foreach (var release in releases)
        {
            _releaseRepository.Add(release);

            var @event = new ReleaseCreatedEvent(release.Id);
            var outboxMessage = OutboxMessage.Create(@event);
            _outboxMessageRepository.Add(outboxMessage);

            await _unitOfWork.SaveChangesIgnoringConflictsAsync(cancellationToken);
        }
    }

    private record UpdateDescriptorKey(
        string AnimeName,
        TranslationType TranslationType,
        string TranslationSourceName);
}
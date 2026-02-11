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

namespace YummyAnimeNotifier.Application.Notifications.Commands.CheckNewEpisodes;

public class CheckNewEpisodesHandler : IRequestHandler<CheckNewEpisodesCommand, Unit>
{
    private readonly YummyAnimeSiteData _yummyAnimeSiteData;
    private readonly IYummyAnimeClient _yummyAnimeClient;
    private readonly IAnimeUpdateParser _animeUpdateParser;
    private readonly AnimeUpdateDescriptorMapper _animeUpdateDescriptorMapper;
    private readonly IAnimeRepository _animeRepository;
    private readonly IAnimeTranslationRepository _animeTranslationRepository;
    private readonly ITranslationSourceRepository _translationSourceRepository;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CheckNewEpisodesHandler(
        IOptions<YummyAnimeSiteData> options,
        IYummyAnimeClient yummyAnimeClient,
        IAnimeUpdateParser animeUpdateParser,
        AnimeUpdateDescriptorMapper animeUpdateDescriptorMapper,
        IAnimeRepository animeRepository,
        IAnimeTranslationRepository animeTranslationRepository,
        ITranslationSourceRepository translationSourceRepository,
        IOutboxMessageRepository outboxMessageRepository,
        IUnitOfWork unitOfWork)
    {
        _yummyAnimeSiteData = options.Value;
        _yummyAnimeClient = yummyAnimeClient;
        _animeUpdateParser = animeUpdateParser;
        _animeUpdateDescriptorMapper = animeUpdateDescriptorMapper;
        _animeRepository = animeRepository;
        _animeTranslationRepository = animeTranslationRepository;
        _outboxMessageRepository = outboxMessageRepository;
        _unitOfWork = unitOfWork;
        _translationSourceRepository = translationSourceRepository;
    }

    public async Task<Unit> Handle(CheckNewEpisodesCommand request, CancellationToken cancellationToken)
    {
        var parsed = await ParseAnimeUpdatesAsync(cancellationToken);

        var updateDescriptors = parsed.Select(_animeUpdateDescriptorMapper.Map).ToArray();

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

        var outboxMessages = GetOutboxMessages(animeTranslations, translationSources, animes, updateDescriptors);
        _outboxMessageRepository.AddRange(outboxMessages);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task<ParsedAnimeUpdate[]> ParseAnimeUpdatesAsync(CancellationToken cancellationToken)
    {
        var uriString = $"{_yummyAnimeSiteData.Domain}{_yummyAnimeSiteData.AnimeUpdatesRelativeLink}";

        var html = await _yummyAnimeClient.GetHtmlStringAsync(new Uri(uriString), cancellationToken);

        var parsed = _animeUpdateParser.Parse(html);

        return parsed;
    }

    private async Task<AnimeTranslation[]> FindAnimeTranslationsAsync(
        AnimeUpdateDescriptor[] updateDescriptors,
        CancellationToken cancellationToken)
    {
        var grouped = updateDescriptors.GroupBy(d => d.AnimeName);

        var animeNames = grouped.Select(g => g.Key).ToArray();
        var existingAnimeNames = await _animeRepository.FilterExistingNamesAsync(animeNames, cancellationToken);
        if (existingAnimeNames.Length == 0)
        {
            return [];
        }

        var existingAnimeNameSet = existingAnimeNames.ToHashSet();
        var tasks = grouped
            .Where(g => existingAnimeNameSet.Contains(g.Key))
            // TODO:
            .Select(g => FindAnimeTranslationsAsync(
                animeName: g.Key,
                updateDescriptors: [.. g],
                cancellationToken))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        return [.. results.SelectMany(r => r)];
    }

    private async Task<AnimeTranslation[]> FindAnimeTranslationsAsync(
        string animeName,
        AnimeUpdateDescriptor[] updateDescriptors,
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

    private OutboxMessage[] GetOutboxMessages(
        AnimeTranslation[] animeTranslations,
        TranslationSource[] translationSources,
        Domain.Entities.Anime[] animes,
        AnimeUpdateDescriptor[] updateDescriptors)
    {
        var translationSourceMap = translationSources.ToDictionary(t => t.Id);
        var animeMap = animes.ToDictionary(a => a.Id);

        var updateDescriptorMap = updateDescriptors
            .GroupBy(
                d => new AnimeUpdateDescriptorKey(
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

        var result = new List<OutboxMessage>();

        foreach (var translation in animeTranslations)
        {
            if (animeMap.TryGetValue(translation.AnimeId, out var anime) == false)
            {
                continue;
            }

            if (translationSourceMap.TryGetValue(translation.TranslationSourceId, out var source) == false)
            {
                continue;
            }

            var key = new AnimeUpdateDescriptorKey(anime.Name, source.Type, source.Name);
            if (updateDescriptorMap.TryGetValue(key, out var episodeNumbers) == false)
            {
                continue;
            }

            var outboxMessages = episodeNumbers
                .Where(n => n > translation.ReleasedEpisodes)
                .Select(n =>
                {
                    var @event = new NewEpisodeDetectedEvent(
                        translation.AnimeId,
                        translation.TranslationSourceId,
                        EpisodeNumber: n);

                    return OutboxMessage.Create(@event);
                })
                .ToArray();

            result.AddRange(outboxMessages);
        }

        return [.. result];
    }

    private record AnimeUpdateDescriptorKey(
        string AnimeName,
        TranslationType TranslationType,
        string TranslationSourceName);
}
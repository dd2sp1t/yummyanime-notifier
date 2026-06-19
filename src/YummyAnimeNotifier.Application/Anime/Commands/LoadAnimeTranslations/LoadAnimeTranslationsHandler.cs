using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using MediatR;
using YummyAnimeNotifier.Application.YummyAnime.Client;
using YummyAnimeNotifier.Application.YummyAnime.Parsers;
using YummyAnimeNotifier.Application.YummyAnime.Mappers;
using YummyAnimeNotifier.Domain.Enums;
using YummyAnimeNotifier.Application.YummyAnime.Mappers.Models;

namespace YummyAnimeNotifier.Application.Anime.Commands.LoadAnimeTranslations;

public class LoadAnimeTranslationsHandler : IRequestHandler<LoadAnimeTranslationsCommand, Unit>
{
    private readonly IYummyAnimeClient _yummyAnimeClient;
    private readonly IAnimeTranslationParser _animeTranslationParser;
    private readonly AnimeTranslationDescriptorMapper _animeTranslationDescriptorMapper;
    private readonly IAnimeRepository _animeRepository;
    private readonly ITranslationSourceRepository _translationSourceRepository;
    private readonly IAnimeTranslationRepository _animeTranslationRepository;
    private readonly IUnitOfWork _unitOfWork;

    public LoadAnimeTranslationsHandler(
        IYummyAnimeClient yummyAnimeClient,
        IAnimeTranslationParser animeTranslationParser,
        AnimeTranslationDescriptorMapper animeTranslationDescriptorMapper,
        IAnimeRepository animeRepository,
        IAnimeTranslationRepository animeTranslationRepository,
        ITranslationSourceRepository translationSourceRepository,
        IUnitOfWork unitOfWork)
    {
        _yummyAnimeClient = yummyAnimeClient;
        _animeTranslationParser = animeTranslationParser;
        _animeTranslationDescriptorMapper = animeTranslationDescriptorMapper;
        _animeRepository = animeRepository;
        _animeTranslationRepository = animeTranslationRepository;
        _translationSourceRepository = translationSourceRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(LoadAnimeTranslationsCommand request, CancellationToken cancellationToken)
    {
        var anime = await _animeRepository.GetAsync(request.AnimeId, cancellationToken);

        var animeTranslationDescriptors = await GetAnimeTranslationDescriptorsAsync(
            anime.ExternalId,
            cancellationToken);

        var translationSources = await GetTranslationSourcesAsync(animeTranslationDescriptors, cancellationToken);

        var joined = animeTranslationDescriptors
            .Join(
                translationSources,
                d => new
                {
                    d.TranslationType,
                    d.TranslationSourceName
                },
                ts => new
                {
                    TranslationType = ts.Type,
                    TranslationSourceName = ts.Name
                },
                (d, ts) => new
                {
                    TranslationSourceId = ts.Id,
                    ReleasedEpisodes = d.MaxEpisodeNumber
                })
            .ToArray();

        var animeTranslations = await _animeTranslationRepository.FindAsync(anime.Id, cancellationToken);
        var existingTranslationSourceIdSet = animeTranslations
            .Select(at => at.TranslationSourceId)
            .ToHashSet();

        foreach (var item in joined)
        {
            if (existingTranslationSourceIdSet.Contains(item.TranslationSourceId))
            {
                continue;
            }

            var @new = AnimeTranslation.Create(
                    anime.Id,
                    item.TranslationSourceId,
                    anime.TotalEpisodes,
                    item.ReleasedEpisodes);

            _animeTranslationRepository.Add(@new);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task<AnimeTranslationDescriptor[]> GetAnimeTranslationDescriptorsAsync(
        int externalId,
        CancellationToken cancellationToken)
    {
        var json = await _yummyAnimeClient.GetVideosJsonAsync(externalId, cancellationToken);

        var parsed = _animeTranslationParser.Parse(json);

        var mapped = parsed
            .Select(_animeTranslationDescriptorMapper.Map)
            .ToArray();

        return mapped;
    }

    private async Task<TranslationSource[]> GetTranslationSourcesAsync(
        AnimeTranslationDescriptor[] descriptors,
        CancellationToken cancellationToken)
    {
        var result = new List<TranslationSource>();

        var grouped = descriptors.GroupBy(
            keySelector: d => d.TranslationType,
            elementSelector: d => d.TranslationSourceName);
        foreach (var group in grouped)
        {
            var translationSources = await GetTranslationSourcesAsync(
                type: group.Key,
                names: [.. group],
                cancellationToken);

            result.AddRange(translationSources);
        }

        return [.. result];
    }

    private async Task<TranslationSource[]> GetTranslationSourcesAsync(
        TranslationType type,
        string[] names,
        CancellationToken cancellationToken)
    {
        var existing = await _translationSourceRepository.FindAsync(type, names, cancellationToken);
        var existingNameSet = existing.Select(ts => ts.Name).ToHashSet();

        var created = names
            .Where(name => existingNameSet.Contains(name) == false)
            .Select(name => TranslationSource.Create(type, name))
            .ToArray();

        _translationSourceRepository.AddRange(created);

        var result = new List<TranslationSource>(capacity: names.Length);
        result.AddRange(existing);
        result.AddRange(created);

        return [.. result];
    }
}
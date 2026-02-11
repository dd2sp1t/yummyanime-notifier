using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using MediatR;
using YummyAnimeNotifier.Application.YummyAnime.Client;
using YummyAnimeNotifier.Application.YummyAnime.Parsers;
using YummyAnimeNotifier.Application.YummyAnime.Mappers;
using YummyAnimeNotifier.Application.Anime.Commands.LoadAnimeTranslations;

namespace YummyAnimeNotifier.Application.Anime.Commands.LoadAnime;

public class LoadAnimeHandler : IRequestHandler<LoadAnimeCommand, Domain.Entities.Anime>
{
    private readonly IYummyAnimeClient _yummyAnimeClient;
    private readonly IAnimeParser _animePageParser;
    private readonly AnimeMapper _animeMapper;
    private readonly IAnimeRepository _animeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMediator _mediator;


    public LoadAnimeHandler(
        IYummyAnimeClient yummyAnimeClient,
        IAnimeParser animePageParser,
        AnimeMapper animeMapper,
        IAnimeRepository animeRepository,
        IUnitOfWork unitOfWork,
        IMediator mediator)
    {
        _animeRepository = animeRepository;
        _unitOfWork = unitOfWork;
        _yummyAnimeClient = yummyAnimeClient;
        _animePageParser = animePageParser;
        _animeMapper = animeMapper;
        _mediator = mediator;
    }

    public async Task<Domain.Entities.Anime> Handle(LoadAnimeCommand request, CancellationToken cancellationToken)
    {
        var anime = await GetAnimeFromSourceAsync(request.SourceUri, cancellationToken);

        _animeRepository.Add(anime);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _mediator.Send(new LoadAnimeTranslationsCommand(anime.Id), cancellationToken);

        return anime;
    }

    private async Task<Domain.Entities.Anime> GetAnimeFromSourceAsync(
        Uri sourceUri,
        CancellationToken cancellationToken)
    {
        var html = await _yummyAnimeClient.GetHtmlStringAsync(sourceUri, cancellationToken);

        var parsed = _animePageParser.Parse(html);

        var anime = _animeMapper.Map(parsed);

        return anime;
    }
}
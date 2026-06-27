using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using MediatR;
using YummyAnimeNotifier.Application.Consumer.Anime.Commands.LoadAnime;
using YummyAnimeNotifier.Application.Consumer.Anime.Commands.LoadAnimeTranslations;
using YummyAnimeNotifier.Application.Exceptions;

namespace YummyAnimeNotifier.Application.Consumer.Subscriptions.Commands.SubscribeToAnime;

public class SubscribeToAnimeHandler : IRequestHandler<SubscribeToAnimeCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IAnimeRepository _animeRepository;
    private readonly IAnimeTranslationRepository _animeTranslationRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;


    public SubscribeToAnimeHandler(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IAnimeRepository animeRepository,
        IAnimeTranslationRepository animeTranslationRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _animeRepository = animeRepository;
        _animeTranslationRepository = animeTranslationRepository;
        _subscriptionRepository = subscriptionRepository;

    }

    public async Task<Unit> Handle(SubscribeToAnimeCommand request, CancellationToken cancellationToken)
    {
        var anime = await GetAnimeAsync(request.SourceLink, cancellationToken);

        var translation = await _animeTranslationRepository.FindAsync(
            anime.Id,
            request.TranslationType,
            request.TranslationSourceName,
            cancellationToken);

        if (translation is null)
        {
            throw new AnimeTranslationNotFoundException(
                anime.Id,
                request.TranslationType,
                request.TranslationSourceName);
        }

        // TODO: rework
        var user = await _userRepository.GetOrCreateByTelegramUserIdAsync(request.TelegramUserId, cancellationToken);

        var existing = await _subscriptionRepository.FindAsync(
            user.Id,
            anime.Id,
            translation.TranslationSourceId,
            cancellationToken);

        if (existing is null)
        {
            var @new = Subscription.Create(user.Id, anime.Id, translation.TranslationSourceId, translation.Status);
            _subscriptionRepository.Add(@new);
        }
        else
        {
            existing.Restore(translation.Status);
            await _subscriptionRepository.UpdateAsync(existing, cancellationToken);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task<Domain.Entities.Anime> GetAnimeAsync(string sourceLink, CancellationToken cancellationToken)
    {
        var sourceUri = new Uri(sourceLink);

        var anime = await _animeRepository.FindBySourceLinkAsync(
            sourceUri.AbsolutePath, // relative source link
            cancellationToken);

        if (anime is null)
        {
            anime = await _mediator.Send(new LoadAnimeCommand(sourceUri), cancellationToken);
        }
        else
        {
            // TODO: limit
            // reload anime translations
            await _mediator.Send(new LoadAnimeTranslationsCommand(anime.Id), cancellationToken);
        }

        return anime;
    }
}
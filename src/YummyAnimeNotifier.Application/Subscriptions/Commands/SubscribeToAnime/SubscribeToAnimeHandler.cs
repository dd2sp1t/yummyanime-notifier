using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using MediatR;
using YummyAnimeNotifier.Application.Anime.Commands.LoadAnime;
using YummyAnimeNotifier.Application.Anime.Commands.LoadAnimeTranslations;

namespace YummyAnimeNotifier.Application.Subscriptions.Commands.SubscribeToAnime;

public class SubscribeToAnimeHandler : IRequestHandler<SubscribeToAnimeCommand, Unit>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IAnimeRepository _animeRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;


    public SubscribeToAnimeHandler(
        IMediator mediator,
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IAnimeRepository animeRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _animeRepository = animeRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Unit> Handle(SubscribeToAnimeCommand request, CancellationToken cancellationToken)
    {
        var anime = await GetAnimeAsync(request.SourceLink, cancellationToken);

        var user = await _userRepository.GetOrCreateByTelegramUserIdAsync(request.TelegramUserId, cancellationToken);

        var existing = await _subscriptionRepository.FindAsync(user.Id, anime.Id, cancellationToken);

        if (existing is null)
        {
            var @new = Subscription.Create(user.Id, anime.Id, anime.Status);
            _subscriptionRepository.Add(@new);
        }
        else
        {
            existing.Restore(anime.Status);
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
            // reload anime translations
            await _mediator.Send(new LoadAnimeTranslationsCommand(anime.Id), cancellationToken);
        }

        return anime;
    }
}
using AniMediaNotifier.Application.Exceptions;
using AniMediaNotifier.Application.Repositories;
using MediatR;

namespace AniMediaNotifier.Application.Subscriptions.Commands.UnsubscribeFromAnime;

public class UnsubscribeFromAnimeHandler : IRequestHandler<UnsubscribeFromAnimeCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IAnimeRepository _animeRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public UnsubscribeFromAnimeHandler(
        IUserRepository userRepository,
        IAnimeRepository animeRepository,
        ISubscriptionRepository subscriptionRepository)
    {
        _userRepository = userRepository;
        _animeRepository = animeRepository;
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Unit> Handle(UnsubscribeFromAnimeCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByTelegramUserIdAsync(request.TelegramUserId, cancellationToken);
        if (user is null)
        {
            throw new UserNotFoundException(request.TelegramUserId);
        }

        var anime = await _animeRepository.FindByRuNameAsync(request.RuName, cancellationToken);
        if (anime == null)
        {
            throw new AnimeNotFoundException(anime.RuName);
        }

        var subscription = await _subscriptionRepository.FindAsync(user.Id, anime.Id, cancellationToken);
        if (subscription is null)
        {
            throw new SubscriptionNotFoundException(user.Id, anime.Id);
        }

        subscription.Cancel();
        await _subscriptionRepository.UpdateAsync(subscription, cancellationToken);

        return Unit.Value;
    }
}
using AniMediaNotifier.Application.Repositories;
using MediatR;

namespace AniMediaNotifier.Application.Subscriptions.Queries.GetUserSubscriptions;

public class GetUserSubscriptionsHandler :
    IRequestHandler<GetUserSubscriptionsQuery, IReadOnlyCollection<GetUserSubscriptionsQueryResultItem>>
{
    private readonly IUserRepository _userRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IAnimeRepository _animeRepository;

    public GetUserSubscriptionsHandler(
        IUserRepository userRepository,
        ISubscriptionRepository subscriptionRepository,
        IAnimeRepository animeRepository)
    {
        _userRepository = userRepository;
        _subscriptionRepository = subscriptionRepository;
        _animeRepository = animeRepository;
    }

    public async Task<IReadOnlyCollection<GetUserSubscriptionsQueryResultItem>> Handle(
        GetUserSubscriptionsQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.FindByTelegramUserIdAsync(request.TelegramUserId, cancellationToken);

        var subscriptions = await _subscriptionRepository.FindByUserIdAsync(user.Id, cancellationToken);

        // TODO: add cache
        var tasks = subscriptions
            .Select(s => _animeRepository.GetAsync(s.AnimeId, cancellationToken))
            .ToArray();
        var animes = await Task.WhenAll(tasks);
        var animeDictionary = animes.ToDictionary(a => a.Id, a => a);

        var results = subscriptions
            .Select(s =>
            {
                var anime = animeDictionary[s.AnimeId];

                return new GetUserSubscriptionsQueryResultItem(
                    anime.RuName,
                    anime.Status,
                    anime.ReleasedEpisodeCount,
                    anime.TotalEpisodeCount,
                    s.IsDeleted
                );
            })
            .ToArray();

        return results;
    }
}
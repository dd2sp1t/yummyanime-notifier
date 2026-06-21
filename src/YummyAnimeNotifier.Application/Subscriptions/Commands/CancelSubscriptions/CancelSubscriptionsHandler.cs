using YummyAnimeNotifier.Application.Persistence.Repositories;
using MediatR;

namespace YummyAnimeNotifier.Application.Subscriptions.Commands.CancelSubscriptions;

public class CancelSubscriptionsHandler : IRequestHandler<CancelSubscriptionsCommand, Unit>
{
    private readonly ISubscriptionRepository _subscriptionRepository;

    public CancelSubscriptionsHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Unit> Handle(CancelSubscriptionsCommand request, CancellationToken cancellationToken)
    {
        await _subscriptionRepository.CancelAsync(
            request.AnimeId,
            request.TranslationSourceId,
            cancellationToken);

        return Unit.Value;
    }
}
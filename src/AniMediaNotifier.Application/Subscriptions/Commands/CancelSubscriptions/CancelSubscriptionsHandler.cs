using AniMediaNotifier.Application.Repositories;
using MediatR;

namespace AniMediaNotifier.Application.Subscriptions.Commands.CancelSubscriptions;

public class CancelSubscriptionsHandler : IRequestHandler<CancelSubscriptionsCommand, Unit>
{
    // TODO: move to config
    private const int BatchSize = 500;
    private readonly ISubscriptionRepository _subscriptionRepository;

    public CancelSubscriptionsHandler(ISubscriptionRepository subscriptionRepository)
    {
        _subscriptionRepository = subscriptionRepository;
    }

    public async Task<Unit> Handle(CancelSubscriptionsCommand request, CancellationToken cancellationToken)
    {
        var skip = 0;

        while (true)
        {
            var batch = await _subscriptionRepository.GetByAnimeId(
                request.AnimeId,
                take: BatchSize,
                skip,
                cancellationToken);

            if (batch.Count == 0)
            {
                break;
            }

            foreach (var sub in batch)
            {
                sub.Cancel();
            }

            await _subscriptionRepository.UpdateAsync(batch, cancellationToken);

            skip += batch.Count;
        }

        return Unit.Value;
    }
}
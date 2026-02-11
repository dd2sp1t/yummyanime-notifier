using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using MediatR;

namespace YummyAnimeNotifier.Application.Subscriptions.Commands.CancelSubscriptions;

public class CancelSubscriptionsHandler : IRequestHandler<CancelSubscriptionsCommand, Unit>
{
    // TODO: move to config
    private const int BatchSize = 500;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelSubscriptionsHandler(ISubscriptionRepository subscriptionRepository, IUnitOfWork unitOfWork)
    {
        _subscriptionRepository = subscriptionRepository;
        _unitOfWork = unitOfWork;
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

            if (batch.Length == 0)
            {
                break;
            }

            foreach (var sub in batch)
            {
                sub.Cancel();
            }

            await _subscriptionRepository.UpdateRangeAsync(batch, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            skip += batch.Length;
        }

        return Unit.Value;
    }
}
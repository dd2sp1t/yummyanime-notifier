using YummyAnimeNotifier.Application.Events;
using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using MediatR;
using YummyAnimeNotifier.Application.Markers;

namespace YummyAnimeNotifier.Application.Worker.Commands.PublishOutboxMessages;

public class PublishOutboxMessagesHandler : IRequestHandler<PublishOutboxMessagesCommand, Unit>, IWorkerAssemblyMarker
{
    // TODO: use IOptions
    private const int MaxCount = 50;
    private readonly IOutboxMessageRepository _outboxMessageRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEventBus _eventBus;

    public PublishOutboxMessagesHandler(
        IOutboxMessageRepository outboxMessageRepository,
        IUnitOfWork unitOfWork,
        IEventBus eventBus)
    {
        _outboxMessageRepository = outboxMessageRepository;
        _unitOfWork = unitOfWork;
        _eventBus = eventBus;
    }

    public async Task<Unit> Handle(PublishOutboxMessagesCommand request, CancellationToken cancellationToken)
    {
        var messages = await _outboxMessageRepository.GetPendingAsync(MaxCount, cancellationToken);

        foreach (var message in messages)
        {
            if (message.TryDeserializeEvent(out var @event, out var error) == false)
            {
                message.MarkAsFailed(error);
                continue;
            }

            var published = await _eventBus.TryPublishAsync(@event, cancellationToken);
            if (published)
            {
                message.MarkAsPublished();
            }
        }

        await _outboxMessageRepository.UpdateRangeAsync(messages, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
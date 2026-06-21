using YummyAnimeNotifier.Application.Notifications.Commands.NotifyUsers;
using YummyAnimeNotifier.Application.Events;
using MediatR;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace YummyAnimeNotifier.Infrastructure.External.MassTransit.Consumers;

public class ReleaseCreated_NotifyUsersConsumer : IConsumer<ReleaseCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReleaseCreated_NotifyUsersConsumer> _logger;

    public ReleaseCreated_NotifyUsersConsumer(
        IMediator mediator,
        ILogger<ReleaseCreated_NotifyUsersConsumer> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ReleaseCreatedEvent> context)
    {
        var @event = context.Message;

        try
        {
            await _mediator.Send(new NotifyUsersCommand(@event.ReleaseId));
        }
        catch (Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                "Failed to notify users about release {ReleaseId}",
                @event.ReleaseId);

            // throw;
        }
    }
}
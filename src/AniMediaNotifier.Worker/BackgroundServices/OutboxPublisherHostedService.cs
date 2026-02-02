using AniMediaNotifier.Application.Events;
using AniMediaNotifier.Application.Repositories;

namespace AniMediaNotifier.Worker.BackgroundServices;

internal class OutboxPublisherHostedService : BackgroundService
{
    private const int MaxCount = 10;
    private const int MillisecondsDelay = 1_000;
    private readonly ILogger<OutboxPublisherHostedService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public OutboxPublisherHostedService(
        ILogger<OutboxPublisherHostedService> logger,
        IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (stoppingToken.IsCancellationRequested == false)
        {
            try
            {
                var scope = _serviceProvider.CreateScope();
                var outboxMessageRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
                var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

                var messages = await outboxMessageRepository.GetPendingAsync(MaxCount, stoppingToken);

                foreach (var message in messages)
                {
                    if (message.TryDeserializeEvent(out var @event, out var error) == false)
                    {
                        message.MarkAsFailed(error);
                        continue;
                    }

                    var published = await eventBus.TryPublishAsync(@event, stoppingToken);
                    if (published)
                    {
                        message.MarkAsPublished();
                    }
                }

                await outboxMessageRepository.UpdateRangeAsync(messages, stoppingToken);
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, "Error processing outbox batch");
            }

            await Task.Delay(MillisecondsDelay, stoppingToken);
        }
    }
}
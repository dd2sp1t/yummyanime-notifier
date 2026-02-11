using YummyAnimeNotifier.Application.Events;
using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;

namespace YummyAnimeNotifier.Worker.BackgroundServices;

internal class OutboxPublisherBackgroundService : BackgroundService
{
    // TODO: use IOptions
    private const int MaxCount = 10;
    private const int MillisecondsDelay = 100_000;
    private readonly ILogger<OutboxPublisherBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public OutboxPublisherBackgroundService(
        ILogger<OutboxPublisherBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var outboxMessageRepository = scope.ServiceProvider.GetRequiredService<IOutboxMessageRepository>();
        var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        while (stoppingToken.IsCancellationRequested == false)
        {
            try
            {
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
                await unitOfWork.SaveChangesAsync(stoppingToken);

            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, "Error processing outbox batch");
            }

            await Task.Delay(MillisecondsDelay, stoppingToken);
        }
    }
}
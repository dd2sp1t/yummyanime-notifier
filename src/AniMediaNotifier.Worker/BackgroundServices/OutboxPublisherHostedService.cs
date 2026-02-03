using AniMediaNotifier.Application.Events;
using AniMediaNotifier.Application.Persistence;
using AniMediaNotifier.Application.Persistence.Repositories;

namespace AniMediaNotifier.Worker.BackgroundServices;

internal class OutboxPublisherHostedService : BackgroundService
{
    // TODO: use IOptions
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
        using var scope = _serviceProvider.CreateScope();

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
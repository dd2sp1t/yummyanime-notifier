using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using YummyAnimeNotifier.Application.Worker.Commands.PublishOutboxMessages;

namespace YummyAnimeNotifier.Application.Worker.BackgroundServices;

internal class OutboxPublisherBackgroundService : BackgroundService
{
    private readonly ILogger<OutboxPublisherBackgroundService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    // TODO: use IOptions
    private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

    public OutboxPublisherBackgroundService(
        ILogger<OutboxPublisherBackgroundService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.Log(LogLevel.Information, $"{nameof(OutboxPublisherBackgroundService)} started");

        using var scope = _serviceScopeFactory.CreateScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        while (stoppingToken.IsCancellationRequested == false)
        {
            try
            {
                _logger.Log(LogLevel.Information, "Processing outbox batch...");

                await mediator.Send(new PublishOutboxMessagesCommand(), stoppingToken);

                _logger.Log(LogLevel.Information, "Outbox batch processed");
            }
            catch (Exception exception)
            {
                _logger.Log(LogLevel.Error, exception, "Error processing outbox batch");
            }

            await Task.Delay(_interval, stoppingToken);
        }

        _logger.Log(LogLevel.Information, $"{nameof(OutboxPublisherBackgroundService)} stopped");
    }
}
using AniMediaNotifier.Application.Notifications.Commands.CheckNewEpisodes;
using MediatR;

namespace AniMediaNotifier.Worker.BackgroundServices
{
    internal class EpisodeTrackingBackgroundService : BackgroundService
    {
        private readonly ILogger<EpisodeTrackingBackgroundService> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        // TODO: use IOptions
        private readonly TimeSpan _interval = TimeSpan.FromMinutes(5);

        public EpisodeTrackingBackgroundService(
            ILogger<EpisodeTrackingBackgroundService> logger,
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Log(LogLevel.Information, $"{nameof(EpisodeTrackingBackgroundService)} started");

            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            while (stoppingToken.IsCancellationRequested == false)
            {
                try
                {
                    _logger.Log(LogLevel.Information, "Checking for new episodes...");

                    await mediator.Send(new CheckNewEpisodesCommand(), stoppingToken);

                    _logger.Log(LogLevel.Information, "Check completed");
                }
                catch (Exception exception)
                {
                    _logger.Log(LogLevel.Error, exception, "Error while checking new episodes");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.Log(LogLevel.Information, $"{nameof(EpisodeTrackingBackgroundService)} stopped");
        }
    }
}
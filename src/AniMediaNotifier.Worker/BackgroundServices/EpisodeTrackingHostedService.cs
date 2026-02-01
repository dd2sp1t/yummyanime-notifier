using AniMediaNotifier.Application.Notifications.Commands.CheckNewEpisodes;
using MediatR;

namespace AniMediaNotifier.Worker.BackgroundServices
{
    internal class EpisodeTrackingHostedService : BackgroundService
    {
        private readonly ILogger<EpisodeTrackingHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval;

        public EpisodeTrackingHostedService(
            ILogger<EpisodeTrackingHostedService> logger,
            IServiceProvider serviceProvider,
            TimeSpan? interval = null)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _interval = interval ?? TimeSpan.FromMinutes(5);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.Log(LogLevel.Information, $"{nameof(EpisodeTrackingHostedService)} started");

            while (stoppingToken.IsCancellationRequested == false)
            {
                try
                {
                    _logger.Log(LogLevel.Information, "Checking for new episodes...");

                    using var scope = _serviceProvider.CreateScope();
                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    await mediator.Send(new CheckNewEpisodesCommand(), stoppingToken);

                    _logger.Log(LogLevel.Information, "Check completed");
                }
                catch (Exception exception)
                {
                    _logger.Log(LogLevel.Error, exception, "Error while checking new episodes");
                }

                await Task.Delay(_interval, stoppingToken);
            }

            _logger.Log(LogLevel.Information, $"{nameof(EpisodeTrackingHostedService)} stopped");
        }
    }
}
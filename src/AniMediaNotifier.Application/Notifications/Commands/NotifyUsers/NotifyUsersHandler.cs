using AniMediaNotifier.Application.Repositories;
using AniMediaNotifier.Domain.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AniMediaNotifier.Application.Notifications.Commands.NotifyUsers;

public class NotifyUsersHandler : IRequestHandler<NotifyUsersCommand, Unit>
{
    private readonly IAnimeRepository _animeRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly ILogger<NotifyUsersHandler> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly SemaphoreSlim _semaphore;

    public NotifyUsersHandler(
        IAnimeRepository animeRepository,
        ISubscriptionRepository subscriptionRepository,
        ILogger<NotifyUsersHandler> logger,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<NotificationSettings> options)
    {
        _animeRepository = animeRepository;
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        var notificationSettings = options.Value;
        _semaphore = new(notificationSettings.MaxParallelism);
    }

    public async Task<Unit> Handle(NotifyUsersCommand request, CancellationToken cancellationToken)
    {
        var anime = await _animeRepository.GetAsync(request.AnimeId, cancellationToken);

        var message = $"{anime.RuName}: вышла серия {request.EpisodeNumber}/{anime.TotalEpisodeCount}";

        var subscriptions = await _subscriptionRepository.FindByAnimeIdAsync(request.AnimeId, cancellationToken);

        var tasks = subscriptions
            .Select(s => TryNotifyAsync(s.UserId, s.AnimeId, message))
            .ToArray();

        await Task.WhenAll(tasks);

        return Unit.Value;
    }

    private async Task TryNotifyAsync(Guid userId, Guid animeId, string message)
    {
        await _semaphore.WaitAsync();

        try
        {
            await NotifyAsync(userId, animeId, message);
        }
        catch (Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                "Failed to notify user {UserId} about anime {AnimeId}",
                userId,
                animeId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task NotifyAsync(Guid userId, Guid animeId, string message)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var notificationSender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

        var notification = Notification.Create(userId, animeId, message);

        await notificationRepository.AddAsync(notification);

        await notificationSender.SendAsync(notification);

        notification.MarkAsSent();

        await notificationRepository.UpdateAsync(notification);
    }
}
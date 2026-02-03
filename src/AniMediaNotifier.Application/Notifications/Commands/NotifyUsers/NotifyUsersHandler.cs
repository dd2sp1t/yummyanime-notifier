using AniMediaNotifier.Application.AniMedia;
using AniMediaNotifier.Application.Notifications.Senders;
using AniMediaNotifier.Application.Persistence;
using AniMediaNotifier.Application.Persistence.Repositories;
using AniMediaNotifier.Domain.Entities;
using AniMediaNotifier.Domain.Enums;
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
    private readonly string _domain;

    public NotifyUsersHandler(
        IAnimeRepository animeRepository,
        ISubscriptionRepository subscriptionRepository,
        ILogger<NotifyUsersHandler> logger,
        IServiceScopeFactory serviceScopeFactory,
        IOptions<NotificationSettings> notificationSettings,
        IOptions<AniMediaSiteData> aniMediaSiteData)
    {
        _animeRepository = animeRepository;
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        _semaphore = new(notificationSettings.Value.MaxParallelism);
        _domain = aniMediaSiteData.Value.Domain;
    }

    public async Task<Unit> Handle(NotifyUsersCommand request, CancellationToken cancellationToken)
    {
        var anime = await _animeRepository.GetAsync(request.AnimeId, cancellationToken);

        var subscriptions = await _subscriptionRepository.FindByAnimeIdAsync(request.AnimeId, cancellationToken);

        var tasks = subscriptions
            .Select(s => TryNotifyAsync(
                s.UserId,
                s.AnimeId,
                anime.RuName,
                url: $"{_domain}{anime.SourceLink}",
                anime.TotalEpisodes,
                request.EpisodeNumber))
            .ToArray();

        await Task.WhenAll(tasks);

        return Unit.Value;
    }

    private async Task TryNotifyAsync(
        Guid userId,
        Guid animeId,
        string ruName,
        string url,
        int? totalEpisodes,
        int episodeNumber)
    {
        await _semaphore.WaitAsync();

        try
        {
            await NotifyAsync(userId, animeId, ruName, url, totalEpisodes, episodeNumber);
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

    private async Task NotifyAsync(
        Guid userId,
        Guid animeId,
        string ruName,
        string url,
        int? totalEpisodes,
        int episodeNumber)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var notificationSender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

        var notification = await notificationRepository.FindAsync(userId, animeId, episodeNumber);
        if (notification is { Status: not NotificationStatus.Pending })
        {
            return;
        }

        if (notification is null)
        {
            notification = Notification.Create(userId, animeId, ruName, url, totalEpisodes, episodeNumber);
            notificationRepository.Add(notification);
            await unitOfWork.SaveChangesAsync();
        }

        var result = await notificationSender.TrySendAsync(notification);
        if (result.Success)
        {
            notification.MarkAsSent();
        }
        else
        {
            notification.MarkAsFailed(result.Error);
        }

        await notificationRepository.UpdateAsync(notification);
        await unitOfWork.SaveChangesAsync();
    }
}
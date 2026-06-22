using YummyAnimeNotifier.Application.Persistence;
using YummyAnimeNotifier.Application.Persistence.Repositories;
using YummyAnimeNotifier.Domain.Entities;
using YummyAnimeNotifier.Domain.Enums;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using YummyAnimeNotifier.Application.YummyAnime;
using YummyAnimeNotifier.Application.Markers;
using YummyAnimeNotifier.Application.Consumer.Notifications.Senders;

namespace YummyAnimeNotifier.Application.Consumer.Notifications.Commands.NotifyUsers;

public class NotifyUsersHandler : IRequestHandler<NotifyUsersCommand, Unit>, IConsumerAssemblyMarker
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
        IOptions<YummyAnimeSiteData> YummyAnimeSiteData)
    {
        _animeRepository = animeRepository;
        _subscriptionRepository = subscriptionRepository;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;

        _semaphore = new(notificationSettings.Value.MaxParallelism);
        _domain = YummyAnimeSiteData.Value.Domain;
    }

    public async Task<Unit> Handle(NotifyUsersCommand request, CancellationToken cancellationToken)
    {
        // TODO: get release
        // var anime = await _animeRepository.GetAsync(request.AnimeId, cancellationToken);

        // var subscriptions = await _subscriptionRepository.FindByAnimeIdAsync(request.AnimeId, cancellationToken);

        // var tasks = subscriptions
        //     .Select(s => TryNotifyAsync(
        //         s.UserId,
        //         s.AnimeId,
        //         anime.Name,
        //         url: $"{_domain}{anime.SourceLink}",
        //         anime.TotalEpisodes,
        //         request.EpisodeNumber))
        //     .ToArray();

        // await Task.WhenAll(tasks);

        return Unit.Value;
    }

    private async Task TryNotifyAsync(
        Guid userId,
        Guid releaseId,
        string animeName,
        string url,
        int? totalEpisodes,
        int episodeNumber,
        string translationSourceName)
    {
        await _semaphore.WaitAsync();

        try
        {
            await NotifyAsync(userId, releaseId, animeName, url, totalEpisodes, episodeNumber, translationSourceName);
        }
        catch (Exception exception)
        {
            _logger.Log(
                LogLevel.Error,
                exception,
                "Failed to notify user {UserId} about release {ReleaseId}",
                userId,
                releaseId);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    private async Task NotifyAsync(
        Guid userId,
        Guid releaseId,
        string animeName,
        string url,
        int? totalEpisodes,
        int episodeNumber,
        string translationSourceName)
    {
        using var scope = _serviceScopeFactory.CreateScope();

        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var notificationRepository = scope.ServiceProvider.GetRequiredService<INotificationRepository>();
        var notificationSender = scope.ServiceProvider.GetRequiredService<INotificationSender>();

        var notification = await notificationRepository.FindAsync(
            userId,
            releaseId,
            episodeNumber,
            translationSourceName);
        if (notification is { Status: not NotificationStatus.Pending })
        {
            return;
        }

        if (notification is null)
        {
            notification = Notification.Create(
                userId,
                releaseId,
                animeName,
                url,
                totalEpisodes,
                episodeNumber,
                translationSourceName);
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
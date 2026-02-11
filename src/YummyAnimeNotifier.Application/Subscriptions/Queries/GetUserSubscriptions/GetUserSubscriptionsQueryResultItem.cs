using YummyAnimeNotifier.Domain.Enums;

namespace YummyAnimeNotifier.Application.Subscriptions.Queries.GetUserSubscriptions;

public record GetUserSubscriptionsQueryResultItem(
    string RuName,
    AnimeStatus Status,
    int? ReleasedEpisodeCount,
    int? TotalEpisodeCount,
    bool IsDeleted);
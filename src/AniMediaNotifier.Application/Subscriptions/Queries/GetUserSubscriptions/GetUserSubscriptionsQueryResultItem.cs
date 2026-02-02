using AniMediaNotifier.Domain.Enums;

namespace AniMediaNotifier.Application.Subscriptions.Queries.GetUserSubscriptions;

public record GetUserSubscriptionsQueryResultItem(
    string RuName,
    AnimeStatus Status,
    int ReleasedEpisodeCount,
    int? TotalEpisodeCount,
    bool IsDeleted);
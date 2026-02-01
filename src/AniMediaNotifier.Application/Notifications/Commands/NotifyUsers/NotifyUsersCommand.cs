using MediatR;

namespace AniMediaNotifier.Application.Notifications.Commands.NotifyUsers;

public record NotifyUsersCommand(
    Guid AnimeId,
    int EpisodeNumber
) : IRequest<Unit>;
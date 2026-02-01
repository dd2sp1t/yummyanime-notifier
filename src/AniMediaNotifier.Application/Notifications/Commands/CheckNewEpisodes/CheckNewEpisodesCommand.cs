using MediatR;

namespace AniMediaNotifier.Application.Notifications.Commands.CheckNewEpisodes;

public record CheckNewEpisodesCommand() : IRequest<Unit>;
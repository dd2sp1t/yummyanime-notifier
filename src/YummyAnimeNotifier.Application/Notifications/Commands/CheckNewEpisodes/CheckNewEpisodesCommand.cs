using MediatR;

namespace YummyAnimeNotifier.Application.Notifications.Commands.CheckNewEpisodes;

public record CheckNewEpisodesCommand() : IRequest<Unit>;
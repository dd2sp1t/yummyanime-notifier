using MediatR;

namespace YummyAnimeNotifier.Application.Worker.Commands.CheckNewEpisodes;

public record CheckNewEpisodesCommand() : IRequest<Unit>;
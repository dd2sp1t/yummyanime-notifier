using MediatR;

namespace YummyAnimeNotifier.Application.Anime.Commands.UpdateAnime;

public record UpdateAnimeCommand(
    Guid AnimeId,
    int EpisodeNumber
) : IRequest<Unit>;
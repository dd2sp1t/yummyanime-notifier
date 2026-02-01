using MediatR;

namespace AniMediaNotifier.Application.Anime.Commands.UpdateAnime;

public record UpdateAnimeCommand(
    Guid AnimeId,
    int EpisodeNumber
) : IRequest<Unit>;
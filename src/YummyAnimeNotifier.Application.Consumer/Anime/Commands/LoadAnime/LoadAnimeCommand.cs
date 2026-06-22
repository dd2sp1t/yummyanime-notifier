using MediatR;

namespace YummyAnimeNotifier.Application.Consumer.Anime.Commands.LoadAnime;

public record LoadAnimeCommand(Uri SourceUri) : IRequest<Domain.Entities.Anime>;
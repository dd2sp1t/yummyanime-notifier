using MediatR;

namespace YummyAnimeNotifier.Application.Anime.Commands.LoadAnimeTranslations;

public record LoadAnimeTranslationsCommand(Guid AnimeId) : IRequest<Unit>;
using MediatR;

namespace YummyAnimeNotifier.Application.Consumer.Anime.Commands.LoadAnimeTranslations;

public record LoadAnimeTranslationsCommand(Guid AnimeId) : IRequest<Unit>;
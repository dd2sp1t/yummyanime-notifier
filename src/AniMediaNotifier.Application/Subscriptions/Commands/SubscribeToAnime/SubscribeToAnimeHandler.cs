using AniMediaNotifier.Application.AniMedia.Client;
using AniMediaNotifier.Application.AniMedia.Mappers;
using AniMediaNotifier.Application.AniMedia.Parsers;
using AniMediaNotifier.Application.Repositories;
using AniMediaNotifier.Domain.Entities;
using MediatR;

namespace AniMediaNotifier.Application.Subscriptions.Commands.SubscribeToAnime;

public class SubscribeToAnimeHandler : IRequestHandler<SubscribeToAnimeCommand, Unit>
{
    private readonly IUserRepository _userRepository;
    private readonly IAnimeRepository _animeRepository;
    private readonly ISubscriptionRepository _subscriptionRepository;
    private readonly IAniMediaClient _aniMediaClient;
    private readonly IAnimePageParser _animePageParser;
    private readonly AnimeMapper _animeMapper;

    public SubscribeToAnimeHandler(
        IUserRepository userRepository,
        IAnimeRepository animeRepository,
        ISubscriptionRepository subscriptionRepository,
        IAniMediaClient aniMediaClient,
        IAnimePageParser animePageParser,
        AnimeMapper animeMapper)
    {
        _userRepository = userRepository;
        _animeRepository = animeRepository;
        _subscriptionRepository = subscriptionRepository;
        _aniMediaClient = aniMediaClient;
        _animePageParser = animePageParser;
        _animeMapper = animeMapper;
    }

    public async Task<Unit> Handle(SubscribeToAnimeCommand request, CancellationToken cancellationToken)
    {
        var anime = await GetAnimeAsync(request.SourceLink, cancellationToken);

        var user = await _userRepository.GetOrCreateByTelegramUserIdAsync(request.TelegramUserId, cancellationToken);

        var existing = await _subscriptionRepository.FindAsync(user.Id, anime.Id, cancellationToken);

        if (existing is null)
        {
            var @new = Subscription.Create(user.Id, anime.Id, anime.Status);

            await _subscriptionRepository.AddAsync(@new, cancellationToken);
        }
        else
        {
            existing.Restore(anime.Status);

            await _subscriptionRepository.UpdateAsync(existing, cancellationToken);
        }

        return Unit.Value;
    }

    private async Task<Domain.Entities.Anime> GetAnimeAsync(string sourceLink, CancellationToken cancellationToken)
    {
        var sourceUri = new Uri(sourceLink);

        var anime = await _animeRepository.FindBySourceLinkAsync(
            sourceUri.AbsolutePath, // relative source link
            cancellationToken);

        if (anime == null)
        {
            anime = await GetAnimeFromSourceAsync(sourceUri, cancellationToken);

            await _animeRepository.AddAsync(anime, cancellationToken);
        }

        return anime;
    }

    private async Task<Domain.Entities.Anime> GetAnimeFromSourceAsync(
        Uri sourceUri,
        CancellationToken cancellationToken)
    {
        var html = await _aniMediaClient.GetHtmlStringAsync(sourceUri, cancellationToken);

        var parsed = _animePageParser.Parse(html);

        var anime = _animeMapper.Map(parsed);

        return anime;
    }
}
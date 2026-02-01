namespace AniMediaNotifier.Domain.Exceptions;

public class InvalidAnimeStateException(string message) : DomainException(message);
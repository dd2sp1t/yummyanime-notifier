namespace YummyAnimeNotifier.Application.Exceptions;

public class AnimeNotFoundException : ApplicationException
{
    public string SourceLink { get; }
    public string Name { get; }

    private AnimeNotFoundException(
        string message,
        string sourceLink = null,
        string name = null)
        : base(message)
    {
        SourceLink = sourceLink;
        Name = name;
    }

    public static AnimeNotFoundException External404(string sourceLink)
        => new("External anime page returned 404", sourceLink: sourceLink);

    public static AnimeNotFoundException ByName(string name)
        => new("Anime was not found by name", name: name);
}
namespace AniMediaNotifier.Application.Exceptions;

public class AnimeNotFoundException : ApplicationException
{
    public string RuName { get; }

    public AnimeNotFoundException(string ruName)
        : base("The anime was not found")
    {
        RuName = ruName;
    }
}
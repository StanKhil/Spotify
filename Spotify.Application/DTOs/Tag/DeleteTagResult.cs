namespace Spotify.Application.DTOs.Tag;

public sealed record DeleteTagResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteTagResult Success() => new(true, []);

    public static DeleteTagResult Failure(params string[] errors) =>
        new(false, errors);
}
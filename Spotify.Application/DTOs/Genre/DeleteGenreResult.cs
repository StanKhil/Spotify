namespace Spotify.Application.DTOs.Genre;

public sealed record DeleteGenreResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteGenreResult Success() => new(true, []);

    public static DeleteGenreResult Failure(params string[] errors) =>
        new(false, errors);
}
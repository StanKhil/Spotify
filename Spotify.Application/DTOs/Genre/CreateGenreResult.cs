namespace Spotify.Application.DTOs.Genre;

public sealed record CreateGenreResult(
    bool Succeeded,
    GenreResponse? Genre,
    IReadOnlyCollection<string> Errors)
{
    public static CreateGenreResult Success(GenreResponse genre) =>
        new(true, genre, []);

    public static CreateGenreResult Failure(params string[] errors) =>
        new(false, null, errors);
}
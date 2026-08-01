namespace Spotify.Application.DTOs.Genre;

public sealed record UpdateGenreResult(
    bool Succeeded,
    GenreResponse? Genre,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateGenreResult Success(GenreResponse genre) =>
        new(true, genre, []);

    public static UpdateGenreResult Failure(params string[] errors) =>
        new(false, null, errors);
}
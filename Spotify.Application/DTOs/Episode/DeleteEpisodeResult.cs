namespace Spotify.Application.DTOs.Episode;

public sealed record DeleteEpisodeResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteEpisodeResult Success() => new(true, []);
    public static DeleteEpisodeResult Failure(params string[] errors) => new(false, errors);
}
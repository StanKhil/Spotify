namespace Spotify.Application.DTOs.Episode;

public sealed record UpdateEpisodeResult(
    bool Succeeded,
    EpisodeResponse? Episode,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateEpisodeResult Success(EpisodeResponse episode) => new(true, episode, []);
    public static UpdateEpisodeResult Failure(params string[] errors) => new(false, null, errors);
}
namespace Spotify.Application.DTOs.Episode;

public sealed record CreateEpisodeResult(
    bool Succeeded,
    EpisodeResponse? Episode,
    IReadOnlyCollection<string> Errors)
{
    public static CreateEpisodeResult Success(EpisodeResponse episode) => new(true, episode, []);
    public static CreateEpisodeResult Failure(params string[] errors) => new(false, null, errors);
}
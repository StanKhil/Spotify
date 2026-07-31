namespace Spotify.Application.DTOs.Podcast;

public sealed record UpdatePodcastResult(
    bool Succeeded,
    PodcastResponse? Podcast,
    IReadOnlyCollection<string> Errors)
{
    public static UpdatePodcastResult Success(PodcastResponse podcast) => new(true, podcast, []);
    public static UpdatePodcastResult Failure(params string[] errors) => new(false, null, errors);
}
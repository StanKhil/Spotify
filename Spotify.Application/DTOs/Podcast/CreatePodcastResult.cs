namespace Spotify.Application.DTOs.Podcast;

public sealed record CreatePodcastResult(
    bool Succeeded,
    PodcastResponse? Podcast,
    IReadOnlyCollection<string> Errors)
{
    public static CreatePodcastResult Success(PodcastResponse podcast) => new(true, podcast, []);
    public static CreatePodcastResult Failure(params string[] errors) => new(false, null, errors);
}
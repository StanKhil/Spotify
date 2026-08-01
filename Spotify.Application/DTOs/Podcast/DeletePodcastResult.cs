namespace Spotify.Application.DTOs.Podcast;

public sealed record DeletePodcastResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeletePodcastResult Success() => new(true, []);
    public static DeletePodcastResult Failure(params string[] errors) => new(false, errors);
}
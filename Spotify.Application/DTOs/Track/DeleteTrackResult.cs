namespace Spotify.Application.DTOs.Track;

public sealed record DeleteTrackResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteTrackResult Success() => new(true, []);
    public static DeleteTrackResult Failure(params string[] errors) => new(false, errors);
}
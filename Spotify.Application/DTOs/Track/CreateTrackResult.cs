namespace Spotify.Application.DTOs.Track;

public sealed record CreateTrackResult(
    bool Succeeded,
    TrackResponse? Track,
    IReadOnlyCollection<string> Errors)
{
    public static CreateTrackResult Success(TrackResponse track) => new(true, track, []);
    public static CreateTrackResult Failure(params string[] errors) => new(false, null, errors);
}
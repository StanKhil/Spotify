namespace Spotify.Application.DTOs.Track;

public sealed record UpdateTrackResult(
    bool Succeeded,
    TrackResponse? Track,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateTrackResult Success(TrackResponse track) => new(true, track, []);
    public static UpdateTrackResult Failure(params string[] errors) => new(false, null, errors);
}
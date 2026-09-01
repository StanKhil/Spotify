namespace Spotify.Application.DTOs.Track;

public sealed record GetLikedTracksResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors,
    TrackResponseCollection? Tracks)
{
    public static GetLikedTracksResult Success(
        TrackResponseCollection tracks)
        => new(true, [], tracks);

    public static GetLikedTracksResult Failure(
        params string[] errors)
        => new(false, errors, null);
}
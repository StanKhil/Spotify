namespace Spotify.Application.DTOs.Track;

public sealed record LikedTracksResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors,
    IEnumerable<LikedTrackResponse>? Tracks)
{
    public static LikedTracksResult Success(
        IEnumerable<LikedTrackResponse> tracks)
        => new(true, [], tracks);

    public static LikedTracksResult Failure(
        params string[] errors)
        => new(false, errors, null);
}
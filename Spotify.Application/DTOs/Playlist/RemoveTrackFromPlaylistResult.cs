namespace Spotify.Application.DTOs.Playlist;

public sealed record RemoveTrackFromPlaylistResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static RemoveTrackFromPlaylistResult Success() => new(true, []);
    public static RemoveTrackFromPlaylistResult Failure(params string[] errors) => new(false, errors);
}
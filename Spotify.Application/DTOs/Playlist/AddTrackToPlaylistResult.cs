namespace Spotify.Application.DTOs.Playlist;

public sealed record AddTrackToPlaylistResult(
    bool Succeeded,
    PlaylistTrackResponse? Track,
    IReadOnlyCollection<string> Errors)
{
    public static AddTrackToPlaylistResult Success(PlaylistTrackResponse track) => new(true, track, []);
    public static AddTrackToPlaylistResult Failure(params string[] errors) => new(false, null, errors);
}
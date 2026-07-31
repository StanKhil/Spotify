namespace Spotify.Application.DTOs.Playlist;

public sealed record UpdatePlaylistResult(
    bool Succeeded,
    PlaylistResponse? Playlist,
    IReadOnlyCollection<string> Errors)
{
    public static UpdatePlaylistResult Success(PlaylistResponse playlist) => new(true, playlist, []);
    public static UpdatePlaylistResult Failure(params string[] errors) => new(false, null, errors);
}
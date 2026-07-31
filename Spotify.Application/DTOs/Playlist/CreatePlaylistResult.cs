namespace Spotify.Application.DTOs.Playlist;

public sealed record CreatePlaylistResult(
    bool Succeeded,
    PlaylistResponse? Playlist,
    IReadOnlyCollection<string> Errors)
{
    public static CreatePlaylistResult Success(PlaylistResponse playlist) => new(true, playlist, []);
    public static CreatePlaylistResult Failure(params string[] errors) => new(false, null, errors);
}
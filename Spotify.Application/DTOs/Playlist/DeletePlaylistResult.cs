namespace Spotify.Application.DTOs.Playlist;

public sealed record DeletePlaylistResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeletePlaylistResult Success() => new(true, []);
    public static DeletePlaylistResult Failure(params string[] errors) => new(false, errors);
}
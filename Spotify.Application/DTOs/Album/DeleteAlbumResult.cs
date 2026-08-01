namespace Spotify.Application.DTOs.Album;

public sealed record DeleteAlbumResult(
    bool Succeeded,
    IReadOnlyCollection<string> Errors)
{
    public static DeleteAlbumResult Success() => new(true, []);

    public static DeleteAlbumResult Failure(params string[] errors) =>
        new(false, errors);
}
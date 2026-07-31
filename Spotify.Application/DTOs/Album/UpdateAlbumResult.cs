namespace Spotify.Application.DTOs.Album;

public sealed record UpdateAlbumResult(
    bool Succeeded,
    AlbumResponse? Album,
    IReadOnlyCollection<string> Errors)
{
    public static UpdateAlbumResult Success(AlbumResponse album) =>
        new(true, album, []);

    public static UpdateAlbumResult Failure(params string[] errors) =>
        new(false, null, errors);
}
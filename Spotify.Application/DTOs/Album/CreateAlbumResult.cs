namespace Spotify.Application.DTOs.Album;

public sealed record CreateAlbumResult(
    bool Succeeded,
    AlbumResponse? Album,
    IReadOnlyCollection<string> Errors)
{
    public static CreateAlbumResult Success(AlbumResponse album) =>
        new(true, album, []);

    public static CreateAlbumResult Failure(params string[] errors) =>
        new(false, null, errors);
}
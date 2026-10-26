using Spotify.Application.DTOs.Album;
using Spotify.Application.Interfaces;

namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record UpdateAuthorAlbumResult(
    bool Succeeded,
    AlbumResponse? Album,
    AuthorUploadFailure FailureType,
    IReadOnlyCollection<string> Errors) : IAuthorUploadResult
{
    public static UpdateAuthorAlbumResult Success(AlbumResponse album) =>
        new(true, album, AuthorUploadFailure.None, []);

    public static UpdateAuthorAlbumResult Failure(AuthorUploadFailure type, params string[] errors) =>
        new(false, null, type, errors);
}
using Spotify.Application.DTOs.Album;
using Spotify.Application.Interfaces;

namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record PublishAuthorAlbumResult(
    bool Succeeded,
    AlbumResponse? Album,
    AuthorUploadFailure FailureType,
    IReadOnlyCollection<string> Errors) : IAuthorUploadResult
{
    public static PublishAuthorAlbumResult Success(AlbumResponse album) =>
        new(true, album, AuthorUploadFailure.None, []);

    public static PublishAuthorAlbumResult Failure(AuthorUploadFailure type, params string[] errors) =>
        new(false, null, type, errors);
}
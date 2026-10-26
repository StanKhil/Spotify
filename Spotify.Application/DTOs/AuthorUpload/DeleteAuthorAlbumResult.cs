using Spotify.Application.Interfaces;

namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record DeleteAuthorAlbumResult(
    bool Succeeded,
    AuthorUploadFailure FailureType,
    IReadOnlyCollection<string> Errors) : IAuthorUploadResult
{
    public static DeleteAuthorAlbumResult Success() =>
        new(true, AuthorUploadFailure.None, []);

    public static DeleteAuthorAlbumResult Failure(AuthorUploadFailure type, params string[] errors) =>
        new(false, type, errors);
}
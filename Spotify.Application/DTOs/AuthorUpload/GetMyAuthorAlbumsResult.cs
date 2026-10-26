using Spotify.Application.DTOs.Album;
using Spotify.Application.Interfaces;

namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record GetMyAuthorAlbumsResult(
    bool Succeeded,
    IReadOnlyCollection<AlbumResponse>? Albums,
    AuthorUploadFailure FailureType,
    IReadOnlyCollection<string> Errors) : IAuthorUploadResult
{
    public static GetMyAuthorAlbumsResult Success(IReadOnlyCollection<AlbumResponse> albums) =>
        new(true, albums, AuthorUploadFailure.None, []);

    public static GetMyAuthorAlbumsResult Failure(AuthorUploadFailure type, params string[] errors) =>
        new(false, null, type, errors);
}
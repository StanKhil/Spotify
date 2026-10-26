using Spotify.Application.Interfaces;

namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record DeleteAuthorTrackResult(
    bool Succeeded,
    AuthorUploadFailure FailureType,
    IReadOnlyCollection<string> Errors) : IAuthorUploadResult
{
    public static DeleteAuthorTrackResult Success() =>
        new(true, AuthorUploadFailure.None, []);

    public static DeleteAuthorTrackResult Failure(AuthorUploadFailure type, params string[] errors) =>
        new(false, type, errors);
}
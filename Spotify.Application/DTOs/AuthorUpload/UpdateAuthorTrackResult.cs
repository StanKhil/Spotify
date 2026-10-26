using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;

namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record UpdateAuthorTrackResult(
    bool Succeeded,
    TrackResponse? Track,
    AuthorUploadFailure FailureType,
    IReadOnlyCollection<string> Errors) : IAuthorUploadResult
{
    public static UpdateAuthorTrackResult Success(TrackResponse track) =>
        new(true, track, AuthorUploadFailure.None, []);

    public static UpdateAuthorTrackResult Failure(AuthorUploadFailure type, params string[] errors) =>
        new(false, null, type, errors);
}
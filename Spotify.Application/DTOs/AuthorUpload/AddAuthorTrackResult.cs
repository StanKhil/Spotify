using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;

namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record AddAuthorTrackResult(
    bool Succeeded,
    TrackResponse? Track,
    AuthorUploadFailure FailureType,
    IReadOnlyCollection<string> Errors) : IAuthorUploadResult
{
    public static AddAuthorTrackResult Success(TrackResponse track) =>
        new(true, track, AuthorUploadFailure.None, []);

    public static AddAuthorTrackResult Failure(AuthorUploadFailure type, params string[] errors) =>
        new(false, null, type, errors);
}
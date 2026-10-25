using Spotify.Application.DTOs.Track;
using Spotify.Application.Interfaces;

namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record GetMyAuthorTracksResult(
    bool Succeeded,
    IReadOnlyCollection<TrackResponse>? Tracks,
    AuthorUploadFailure FailureType,
    IReadOnlyCollection<string> Errors) : IAuthorUploadResult
{
    public static GetMyAuthorTracksResult Success(IReadOnlyCollection<TrackResponse> tracks) =>
        new(true, tracks, AuthorUploadFailure.None, []);

    public static GetMyAuthorTracksResult Failure(AuthorUploadFailure type, params string[] errors) =>
        new(false, null, type, errors);
}
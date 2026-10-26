using Spotify.Application.DTOs.AuthorUpload;

namespace Spotify.Application.Interfaces;

public interface IAuthorUploadResult
{
    bool Succeeded { get; }
    AuthorUploadFailure FailureType { get; }
    IReadOnlyCollection<string> Errors { get; }
}
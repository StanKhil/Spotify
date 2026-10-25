namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record AuthorTrackUploadRequest(
    Guid AlbumId,
    string Name,
    string? Description,
    string? GenreId,
    Guid? MoodId,
    bool IsAdult,
    bool IsDraft,
    UploadedFile Audio);
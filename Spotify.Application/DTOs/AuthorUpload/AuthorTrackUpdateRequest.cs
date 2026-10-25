namespace Spotify.Application.DTOs.AuthorUpload;

public sealed record AuthorTrackUpdateRequest(
    Guid TrackId,
    string Name,
    string? Description,
    string? GenreId,
    Guid? MoodId,
    bool IsAdult,
    bool IsDraft);
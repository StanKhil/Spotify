namespace Spotify.Application.DTOs.Track;

public sealed record TrackResponse(
    string Id,
    string Name,
    string? Description,
    int DurationSeconds,
    string? AlbumId,
    string? MoodId,
    string? GenreId,
    long PlaysNumber,
    bool IsAdult,
    bool IsDraft,
    Guid? AudioItemId,
    Guid? ImageItemId,
    IReadOnlyCollection<string> TagIds,
    DateTime CreatedAt,
    string? AudioUrl,
    IReadOnlyCollection<string>? AuthorIds = null,
    string? ImageUrl = null);
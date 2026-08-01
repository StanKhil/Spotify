namespace Spotify.Application.DTOs.Track;

public sealed record TrackResponse(
    Guid Id,
    string Name,
    string? Description,
    int DurationSeconds,
    Guid AlbumId,
    Guid? MoodId,
    string? GenreId,
    long PlaysNumber,
    bool IsAdult,
    bool IsDraft,
    Guid? AudioItemId,
    Guid? ImageItemId,
    IReadOnlyCollection<string> TagIds,
    DateTime CreatedAt);
namespace Spotify.Application.DTOs.Album;

public sealed record AlbumResponse(
    Guid Id,
    string Name,
    string? Description,
    int DurationSeconds,
    Guid? CoverImageId,
    bool IsDraft,
    string? GenreId,
    DateTime CreatedAt);
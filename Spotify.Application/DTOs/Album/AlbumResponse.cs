namespace Spotify.Application.DTOs.Album;

public sealed record AlbumResponse(
    string Id,
    string Name,
    string? Description,
    int DurationSeconds,
    Guid? CoverImageId,
    bool IsDraft,
    string? GenreId,
    DateTime? CreatedAt,
    string? ImageUrl);
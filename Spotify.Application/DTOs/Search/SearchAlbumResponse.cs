namespace Spotify.Application.DTOs.Search;

public sealed record SearchAlbumResponse(
    string Id,
    string Name,
    string? Description,
    int DurationSeconds,
    string Provider,
    string? ArtistName,
    string? ImageUrl,
    int TracksCount,
    DateTime? ReleaseDate);

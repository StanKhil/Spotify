namespace Spotify.Application.DTOs.Search;

public sealed record SearchTrackResponse(
    string Id,
    string Name,
    string? Description,
    int DurationSeconds,
    string Provider,
    string? AlbumId,
    string? AlbumName,
    string? ArtistName,
    string? ImageUrl,
    string? PlaybackUrl);

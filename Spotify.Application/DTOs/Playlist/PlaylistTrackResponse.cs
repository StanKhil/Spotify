namespace Spotify.Application.DTOs.Playlist;

public sealed record PlaylistTrackResponse(
    Guid Id,
    string Name,
    string? Album,
    int DurationSeconds,
    string? ArtistName,
    string? ImageUrl,
    string? AudioUrl,
    int Position,
    DateTime AddedAt);
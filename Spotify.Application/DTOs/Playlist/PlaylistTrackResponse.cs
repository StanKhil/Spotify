namespace Spotify.Application.DTOs.Playlist;

public sealed record PlaylistTrackResponse(
    Guid Id,
    string Name,
    string? Album,
    int DurationSeconds,
    string? ArtistName,
    string? ImageUrl,
    int Position,
    DateTime AddedAt);
namespace Spotify.Application.DTOs.Playlist;

public sealed record PlaylistTrackResponse(
    Guid TrackId,
    string TrackName,
    string? AlbumName,
    int Seconds,
    string? ArtistName,
    string? CoverImageUrl,
    int Position,
    DateTime AddedAt);
namespace Spotify.Application.DTOs.Playlist;

public sealed record PlaylistTrackResponse(
    Guid TrackId,
    string TrackName,
    int Position,
    DateTime AddedAt);
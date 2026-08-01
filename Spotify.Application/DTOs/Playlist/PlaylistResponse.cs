namespace Spotify.Application.DTOs.Playlist;

public sealed record PlaylistResponse(
    Guid Id,
    string Name,
    Guid ApplicationUserId,
    int TracksCount);
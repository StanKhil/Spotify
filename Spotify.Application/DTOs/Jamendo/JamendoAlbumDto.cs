namespace Spotify.Application.DTOs.Jamendo;

public sealed record JamendoAlbumDto(
    string Id,
    string Name,
    string ArtistId,
    string ArtistName,
    string ImageUrl,
    int TracksCount,
    DateTime? ReleaseDate);
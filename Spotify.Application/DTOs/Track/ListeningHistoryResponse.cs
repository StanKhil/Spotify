namespace Spotify.Application.DTOs.Track
{
    public sealed record ListeningHistoryResponse(
        Guid Id,
        Guid TrackId,
        string TrackName,
        string ArtistName,
        string AlbumName,
        string DurationSeconds,
        string? AudioUrl,
        string? imageUrl
        );

}
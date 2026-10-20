namespace Spotify.Application.DTOs.Track
{
    public sealed record ListeningHistoryResponse(
        Guid Id,
        // Guid TrackId,
        string Name,
        string ArtistName,
        string AlbumName,
        string DurationSeconds,
        string? AudioUrl,
        string? ImageUrl
        );

}
namespace Spotify.Application.DTOs.Track
{
    public sealed record TrackActionResponse(
        Guid TrackId,
        long PlaysNumber,
        bool IsLiked);
}

namespace Spotify.Application.DTOs.Track
{
    public sealed record TrackResponseCollection(
        IReadOnlyCollection<TrackResponse> Tracks,
        int TotalCount,
        int PageNumber
    );

}

using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public sealed record TrackRecommendationResponse(
        string Id,
        string Name,
        int DurationSeconds,
        string? AlbumId,
        string? AuthorName,
        string? AudioUrl,
        string? ImageUrl
    );
}

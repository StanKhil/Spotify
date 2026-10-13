using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public sealed record TrackPopularResponse(
        string Id,
        string Name,
        int DurationSeconds,
        string? AlbumId,
        string? AudioUrl,
        string? ImageUrl
    );
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public sealed record LikedTrackResponse(
        Guid Id,
        string? ExternalId,
        string Name,
        string ArtistName,
        string Album,
        string LikedAt,
        int DurationSeconds,
        string? ImageUrl,
        string? AudioUrl,
        int Order
        );

}

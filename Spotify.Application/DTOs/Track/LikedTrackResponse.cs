using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public sealed record LikedTrackResponse(
        Guid Id,
        string? ExternalId,
        string Name,
        string Author,
        string Album,
        string LikedAt,
        int DurationSeconds);

}

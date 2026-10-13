using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public sealed record TrackAlbumResponse(
        string Id,
        string Name,
        DateTime? ReleaseDate,
        string? ImageUrl
    );
}

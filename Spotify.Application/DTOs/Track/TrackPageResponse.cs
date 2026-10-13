using Microsoft.AspNetCore.Http.HttpResults;
using Spotify.Application.DTOs.Album;
using Spotify.Application.DTOs.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public record TrackPageResponse(
        string Name,
        string? Description,
        List<string> AuthorsNames,
        List<string> AuthorsIds,
        string AlbumName,
        string? AlbumId,
        string? ImageUrl,
        string? AudioUrl,
        long Listeners,
        int DurationSeconds,
        DateTime CreatedAt,
        SearchPage<TrackResponse> Recommendation,
        SearchPage<TrackResponse> PopularTracksByAuthor,
        SearchPage<AlbumResponse> AlbumsByAuthor
    );
}

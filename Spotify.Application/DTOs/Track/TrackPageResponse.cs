using Microsoft.AspNetCore.Http.HttpResults;
using Spotify.Application.DTOs.Album;
using Spotify.Application.DTOs.Author;
using Spotify.Application.DTOs.Search;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public record TrackPageResponse(
        string Name,
        string? Description,
        List<AuthorResponse> Authors,
        string AlbumName,
        string? AlbumId,
        string? ImageUrl,
        string? AudioUrl,
        long Listeners,
        int DurationSeconds,
        DateTime CreatedAt,
        SearchPage<TrackRecommendationResponse> Recomendation,
        SearchPage<TrackPopularResponse> PopularTracksByAuthor,
        SearchPage<TrackAlbumResponse> AlbumsByAuthor
    );
}

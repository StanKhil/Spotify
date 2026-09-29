using Microsoft.AspNetCore.Http.HttpResults;
using Spotify.Application.DTOs.Album;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Track
{
    public sealed record TrackPageResponse(
        string Name,
        string? Description,
        IReadOnlyCollection<string> AuthorsNames,
        IReadOnlyCollection<string> AuthorsIds,
        string AlbumName,
        string? ImageUrl,
        string? AudioUrl,
        long PlaysNumber,
        DateTime CreatedAt,
        IReadOnlyCollection<TrackResponse> Recomendation,
        IReadOnlyCollection<TrackResponse> PopularTracksByAuthor,
        IReadOnlyCollection<AlbumResponse> AlbumsByAuthor
        );
}

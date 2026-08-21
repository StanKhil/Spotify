using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Application.DTOs.Jamendo
{
    public sealed record JamendoAlbumTrackDto(
        string Id,
    string Name,
    string ArtistId,
    string ArtistName,
    string ImageUrl,
    int TracksCount,
    DateTime? ReleaseDate,
    IReadOnlyCollection<JamendoTrackDto> Tracks); 
}

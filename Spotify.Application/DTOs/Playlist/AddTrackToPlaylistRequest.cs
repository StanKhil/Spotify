using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Playlist;

public sealed class AddTrackToPlaylistRequest
{
    [Required]
    public string TrackId { get; init; }
}
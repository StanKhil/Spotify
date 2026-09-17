using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Playlist;

public sealed class AddTrackToPlaylistRequest
{
    [Required]
    public Guid TrackId { get; init; }
}
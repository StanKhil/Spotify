using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Playlist;

public sealed class UpdatePlaylistRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;
}
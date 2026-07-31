using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Playlist;

public sealed class CreatePlaylistRequest
{
    [Required]
    [MaxLength(150)]
    public string Name { get; init; } = string.Empty;

    [Required]
    public Guid ApplicationUserId { get; init; }
}
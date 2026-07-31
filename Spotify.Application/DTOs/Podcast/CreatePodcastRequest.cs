using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Podcast;

public sealed class CreatePodcastRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Required]
    [MaxLength(2000)]
    public string Description { get; init; } = string.Empty;
}
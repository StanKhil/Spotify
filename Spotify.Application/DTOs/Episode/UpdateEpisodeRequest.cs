using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Episode;

public sealed class UpdateEpisodeRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    public Guid? ImageItemId { get; init; }
}
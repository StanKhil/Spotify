using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Tag;

public sealed class CreateTagRequest
{
    [Required]
    [MaxLength(50)]
    public string Id { get; init; } = string.Empty;
}
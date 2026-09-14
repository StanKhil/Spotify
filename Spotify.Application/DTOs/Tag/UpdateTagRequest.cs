using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Tag;

public sealed class UpdateTagRequest
{
    [Required]
    [MaxLength(50)]
    public string NewId { get; init; } = string.Empty;
}
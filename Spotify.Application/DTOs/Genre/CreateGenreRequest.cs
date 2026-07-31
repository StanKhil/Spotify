using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Genre;

public sealed class CreateGenreRequest
{
    [Required]
    [MaxLength(50)]
    public string Id { get; init; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;
}
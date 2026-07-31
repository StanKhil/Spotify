using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Genre;

public sealed class UpdateGenreRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;
}
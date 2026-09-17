using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Author;

public sealed class UpdateAuthorRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [Range(0, int.MaxValue)]
    public int MonthList { get; init; }

    [MaxLength(2000)]
    public string? Bio { get; init; }

    public Guid? BioImageItemId { get; init; }
}
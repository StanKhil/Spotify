using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Audiobook;

public sealed class CreateAudiobookRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    [Required]
    public Guid AudioItemId { get; init; }

    [Required]
    public Guid AuthorContentId { get; init; }

    public string? GenreId { get; init; }
}
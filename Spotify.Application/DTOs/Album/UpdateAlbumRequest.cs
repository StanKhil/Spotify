using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Album;

public sealed class UpdateAlbumRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    [Required]
    public Guid CoverImageId { get; init; }

    public string? GenreId { get; init; }

    public bool IsDraft { get; init; }
}
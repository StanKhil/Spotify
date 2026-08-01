using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Track;

public sealed class CreateTrackRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; init; }

    [Required]
    public Guid AlbumId { get; init; }

    public Guid? MoodId { get; init; }

    public string? GenreId { get; init; }

    [Required]
    public Guid AudioItemId { get; init; }

    public Guid? ImageItemId { get; init; }

    public bool IsAdult { get; init; }

    public bool IsDraft { get; init; } = true;

    public IReadOnlyCollection<string> TagIds { get; init; } = [];
}
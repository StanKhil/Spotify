using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Mood;

public sealed class CreateMoodRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    public Guid? MoodImageId { get; init; }
}
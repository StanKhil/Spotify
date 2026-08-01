using System.ComponentModel.DataAnnotations;

namespace Spotify.Application.DTOs.Track;

public sealed class BatchDeleteTracksRequest
{
    [Required]
    [MinLength(1)]
    public IReadOnlyCollection<Guid> TrackIds { get; init; } = [];
}
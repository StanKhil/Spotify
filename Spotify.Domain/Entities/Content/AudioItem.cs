using Spotify.Domain.Enumerations;

namespace Spotify.Domain.Entities.Content;

public class AudioItem
{
    public Guid Id { get; set; }
    public AudioProvider Provider { get; set; }
    public string? StorageKey { get; set; }
    public string? ContentType { get; set; }
    public int? BitrateKbps { get; set; }
    public string? LicenseUrl { get; set; }
    public bool IsDownloadAllowed { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

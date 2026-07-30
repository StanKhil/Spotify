namespace Spotify.Domain.Entities.Content;

public class Track : AudioContent
{
    public Guid? AlbumId { get; set; }
    public Guid? MoodId { get; set; }
    public Mood? Mood { get; set; }
    public long PlaysNumber { get; set; }
    public bool IsAdult { get; set; }
    public bool IsDraft { get; set; } = true;
    public ICollection<TrackTag> TrackTags { get; set; } = new List<TrackTag>();
    public Album Album { get; set; } = null!;
    public ICollection<PlaylistTrack> PlaylistTracks { get; set; } = new List<PlaylistTrack>();
}

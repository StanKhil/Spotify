namespace Spotify.Domain.Entities.Content
{
    public class Album : AudioContent
    {
        public bool IsDraft { get; set; }
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}
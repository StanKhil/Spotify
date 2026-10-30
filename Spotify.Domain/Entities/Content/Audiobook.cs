namespace Spotify.Domain.Entities.Content
{
    public class Audiobook : AudioContent
    {
        public ICollection<Episode> Episodes { get; set; } = [];
    }
}
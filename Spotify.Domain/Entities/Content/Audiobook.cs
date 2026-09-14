namespace Spotify.Domain.Entities.Content
{
    public class Audiobook : AudioContent
    {
        public Guid AuthorContentId { get; set; }
        public AuthorContent AuthorContent { get; set; } = null!;

        public ICollection<Episode> Episodes { get; set; } = [];
    }
}
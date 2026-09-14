using Spotify.Domain.Entities.Content;

namespace Spotify.Domain.Entities.Content
{
    public class PodcastAuthor
    {
        public Guid PodcastId { get; set; }
        public Podcast Podcast { get; set; } = null!;

        public Guid AuthorId { get; set; }
        public Author Author { get; set; } = null!;
    }
}
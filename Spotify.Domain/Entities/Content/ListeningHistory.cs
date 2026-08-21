using Spotify.Domain.Entities.User;

namespace Spotify.Domain.Entities.Content
{
    public class ListeningHistory
    {
        public Guid Id { get; set; }

        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public Guid AuthorContentId { get; set; }
        public AuthorContent AuthorContent { get; set; } = null!;

        public int ListenedSeconds { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}

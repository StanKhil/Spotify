using Spotify.Domain.Entities.User;

namespace Spotify.Domain.Entities.Content
{
    public class Author
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string? ExternalAuthorId { get; set; }

        public Guid? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        public ICollection<AuthorContentAuthor> AuthoredContent { get; set; } = [];
        public ICollection<AuthorSubscription> Followers { get; set; } = [];
    }
}

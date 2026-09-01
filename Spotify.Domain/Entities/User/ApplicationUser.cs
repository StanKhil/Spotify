using Microsoft.AspNetCore.Identity;
using Spotify.Domain.Entities.Content;

namespace Spotify.Domain.Entities.User
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;
        public Guid SettingsId { get; set; }
        public Settings Settings { get; set; } = null!;

        public UserProfile Profile { get; set; } = null!;
        public Author? Author { get; set; }
        public ICollection<ListeningHistory> ListeningHistory { get; set; }
    = new List<ListeningHistory>();
        public ICollection<Playlist> Playlists { get; set; } = [];
        public ICollection<Like> Likes { get; set; } = [];
        public ICollection<AuthorSubscription> AuthorSubscriptions { get; set; } = [];

    }
}

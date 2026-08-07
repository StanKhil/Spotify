using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Spotify.Domain.Entities.Content;

namespace Spotify.Domain.Entities.User
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid SubscriptionId { get; set; }
        public Subscription Subscription { get; set; } = null!;
        public Boolean IsAuthor { get; set; } = false;
        public Guid SettingsId { get; set; }
        public Settings Settings { get; set; } = null!;

        public UserProfile Profile { get; set; } = null!;

        public ICollection<AuthorContent> AuthoredContent { get; set; } = [];
        public ICollection<Playlist> Playlists { get; set; } = [];
        public ICollection<Like> Likes { get; set; } = [];
    }
}

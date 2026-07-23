using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace Spotify.Domain.Entities.Content
{
    public class UserAccess : IdentityUser<Guid>
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        // In IdentityUser class!

        //public string Login { get; set; } = null!;        // UserLogin, NormalizedUserLogin    

        // string Salt { get; set; } = null!;

        //public string DerivedKey { get; set; } = null!;   // PasswordHash
        public Guid SubscriptionId { get; set; }

        [ForeignKey(nameof(SubscriptionId))]
        public Subscription Subscription { get; set; } = null!;

        public Guid SettingsId { get; set; }

        [ForeignKey(nameof(SettingsId))]
        public Settings Settigns { get; set; } = null!;

        [ForeignKey(nameof(UserId))]
        public UserData User { get; set; } = null!;

        [ForeignKey(nameof(RoleId))]
        public UserRole Role { get; set; } = null!;

        public ICollection<AuthorContent> AuthoredContent { get; set; } = [];
        public ICollection<Playlist> Playlists { get; set; } = [];
        public ICollection<Like> Likes { get; set; } = [];
    }
}

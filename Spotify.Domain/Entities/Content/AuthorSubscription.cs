using Spotify.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class AuthorSubscription
    {
        public Guid Id { get; set; }

        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public Guid AuthorId { get; set; }
        public Author Author { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

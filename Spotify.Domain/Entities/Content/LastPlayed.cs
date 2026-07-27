using System;

namespace Spotify.Domain.Entities.Content
{
    public class LastPlayed
    {
        public Guid Id { get; set; }

        public Guid ApplicationUserId { get; set; }
        public ApplicationUser ApplicationUser { get; set; } = null!;

        public Guid AudioContentId { get; set; }
        public AudioContent AudioContent { get; set; } = null!;

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}
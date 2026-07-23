using System;

namespace Spotify.Domain.Entities.Content
{
    public class LastPlayed
    {
        public Guid Id { get; set; }

        public Guid UserAccessId { get; set; }
        public UserAccess UserAccess { get; set; } = null!;

        public Guid AudioContentId { get; set; }
        public AudioContent AudioContent { get; set; } = null!;

        public DateTime PlayedAt { get; set; } = DateTime.UtcNow;
    }
}
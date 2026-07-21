using System;

namespace Spotify.Domain.Entities.Content
{
    public class TrackTag
    {
        public Guid TrackId { get; set; }
        public Track Track { get; set; } = null!;

        public string TagId { get; set; } = null!;
        public Tag Tag { get; set; } = null!;
    }
}
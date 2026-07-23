using System;
using System.Collections.Generic;

namespace Spotify.Domain.Entities.Content
{
    public class Track : AudioContent
    {
        public Guid? AlbumId { get; set; }
        public Guid? MoodId { get; set; }
        public Mood? Mood { get; set; }
        public long PlaysNumber { get; set; } = 0;
        public bool IsAdult { get; set; }
        public bool IsDraft { get; set; } = true;
        public ICollection<TrackTag> TrackTags { get; set; } = new List<TrackTag>();
        public Guid? PlaylistId { get; set; }
        public Playlist? Playlist { get; set; }
        public Album Album { get; set; } = null!;
    }
}
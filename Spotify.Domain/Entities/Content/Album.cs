using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class Album : AudioContent
    {
        public Guid CoverImageId { get; set; }

        public CoverImage CoverImage { get; set; } = null!;

        public bool IsDraft { get; set; }
        public ICollection<Track> Tracks { get; set; } = new List<Track>();
    }
}

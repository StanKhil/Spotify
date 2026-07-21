using System;
using System.Collections.Generic;

namespace Spotify.Domain.Entities.Content
{
    public class LastPlayed
    {
        public Guid Id { get; set; }
        public Guid UserAccessId { get; set; }
        public ICollection<AudioContent> AudioContents { get; set; } = new List<AudioContent>();
    }
}
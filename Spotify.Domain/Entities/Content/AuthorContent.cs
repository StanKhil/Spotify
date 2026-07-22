using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class AuthorContent
    {
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }
        public UserData Author { get; set; } = null!;

        public Guid ItemId { get; set; }
        public AudioContent Item { get; set; } = null!;
    }
}

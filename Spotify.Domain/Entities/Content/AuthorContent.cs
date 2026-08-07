using Spotify.Domain.Entities.User;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class AuthorContent
    {
        public Guid Id { get; set; }

        public Guid ItemId { get; set; }

        [ForeignKey(nameof(ItemId))]
        public AudioContent Item { get; set; } = null!;

        // Many-to-many: one AuthorContent can have many Authors
        public ICollection<AuthorContentAuthor> Authors { get; set; } = [];
    }
}

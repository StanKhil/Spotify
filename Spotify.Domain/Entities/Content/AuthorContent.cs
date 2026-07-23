using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class AuthorContent
    {
        public Guid Id { get; set; }

        public Guid AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public UserAccess Author { get; set; } = null!;

        public Guid ItemId { get; set; }

        [ForeignKey(nameof(ItemId))]
        public AudioContent Item { get; set; } = null!;
    }
}

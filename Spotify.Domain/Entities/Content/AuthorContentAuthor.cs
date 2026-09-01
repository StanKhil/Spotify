using Spotify.Domain.Entities.User;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spotify.Domain.Entities.Content
{
    public class AuthorContentAuthor
    {
        public Guid AuthorContentId { get; set; }

        [ForeignKey(nameof(AuthorContentId))]
        public AuthorContent AuthorContent { get; set; } = null!;

        public Guid AuthorId { get; set; }

        [ForeignKey(nameof(AuthorId))]
        public Author Author { get; set; } = null!;
    }
}

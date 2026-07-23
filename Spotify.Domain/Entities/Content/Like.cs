using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class Like
    {
        public Guid Id { get; set; }
        public Guid AuthorContentId { get; set; }
        public Guid UserAccessId { get; set; }

        [ForeignKey(nameof(AuthorContentId))]
        public AuthorContent AuthorContent { get; set; } = null!;
    }
}

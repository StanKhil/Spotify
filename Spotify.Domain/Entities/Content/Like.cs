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
        public Guid ApplicationUserId { get; set; }

        [ForeignKey(nameof(ApplicationUserId))]
        public ApplicationUser ApplicationUser { get; set; } = null!;

        [ForeignKey(nameof(AuthorContentId))]
        public AuthorContent AuthorContent { get; set; } = null!;
    }
}

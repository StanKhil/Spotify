using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Spotify.Domain.Entities.Content
{
    public class Subscription
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        [Column(TypeName = "decimal(13,2)")]
        public decimal Price { get; set; }

        public List<UserAccess> UserAccesses { get; set; } = [];
    }
}

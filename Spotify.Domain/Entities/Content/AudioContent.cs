using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Spotify.Domain.Entities.Content
{
    public abstract class AudioContent
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public int DurationSeconds { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeletedAt { get; set; }
        public bool IsForAdult { get; set; }

        public Guid? ImageItemId { get; set; }

        [ForeignKey(nameof(ImageItemId))]
        public ImageItem? ImageItem { get; set; }

        public Guid AudioItemId { get; set; }

        [ForeignKey(nameof(AudioItemId))]
        public AudioItem AudioItem { get; set; } = null!;

        public string? GenreId { get; set; }

        [ForeignKey(nameof(GenreId))]
        public Genre? Genre { get; set; }

        public ICollection<LastPlayed>? LastPlayedEntries { get; set; }

        public ICollection<AuthorContent> Authors { get; set; } = new List<AuthorContent>();
    }
}
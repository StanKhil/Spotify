using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    internal class TrackTagConfiguration : IEntityTypeConfiguration<TrackTag>
    {
        public void Configure(EntityTypeBuilder<TrackTag> builder)
        {
            builder.HasKey(x => new { x.TrackId, x.TagId });

            builder.HasOne(x => x.Track)
                .WithMany(t => t.TrackTags)
                .HasForeignKey(x => x.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Tag)
                .WithMany()
                .HasForeignKey(x => x.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

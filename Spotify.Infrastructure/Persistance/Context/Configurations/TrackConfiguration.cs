using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class TrackConfiguration : IEntityTypeConfiguration<Track>
    {
        public void Configure(EntityTypeBuilder<Track> builder)
        {
            builder.HasOne(x => x.Album)
                .WithMany(x => x.Tracks)
                .HasForeignKey(x => x.AlbumId)
                .OnDelete(DeleteBehavior.NoAction);
            builder.HasMany(x => x.TrackTags)
                .WithOne(x => x.Track)
                .HasForeignKey(x => x.TrackId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Mood)
                .WithMany()
                .HasForeignKey(x => x.MoodId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Playlist)
                .WithMany(x => x.Tracks)
                .HasForeignKey(x => x.PlaylistId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(x => x.PlaysNumber)
                .IsRequired();

            builder.Property(x => x.IsAdult)
                .IsRequired();

            builder.Property(x => x.IsDraft)
                .IsRequired();
        }
    }
}

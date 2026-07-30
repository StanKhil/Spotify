using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations;

public sealed class PlaylistTrackConfiguration : IEntityTypeConfiguration<PlaylistTrack>
{
    public void Configure(EntityTypeBuilder<PlaylistTrack> builder)
    {
        builder.HasKey(x => new { x.PlaylistId, x.TrackId });
        builder.Property(x => x.Position).IsRequired();
        builder.Property(x => x.AddedAt).IsRequired();
        builder.HasIndex(x => new { x.PlaylistId, x.Position }).IsUnique();

        builder.HasOne(x => x.Playlist)
            .WithMany(x => x.PlaylistTracks)
            .HasForeignKey(x => x.PlaylistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Track)
            .WithMany(x => x.PlaylistTracks)
            .HasForeignKey(x => x.TrackId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations;

public sealed class AudioItemConfiguration : IEntityTypeConfiguration<AudioItem>
{
    public void Configure(EntityTypeBuilder<AudioItem> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Provider).IsRequired();
        builder.Property(x => x.StorageKey).HasMaxLength(1024);
        builder.Property(x => x.ExternalContentId).HasMaxLength(128);
        builder.Property(x => x.ContentType).HasMaxLength(100);
        builder.Property(x => x.LicenseUrl).HasMaxLength(2048);

        builder.HasIndex(x => new { x.Provider, x.ExternalContentId })
            .IsUnique()
            .HasFilter("[ExternalContentId] IS NOT NULL");
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations;

public sealed class LastPlayedConfiguration : IEntityTypeConfiguration<LastPlayed>
{
    public void Configure(EntityTypeBuilder<LastPlayed> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.PositionSeconds).IsRequired();
        builder.Property(x => x.PlayedAt).IsRequired();
        builder.Property(x => x.UpdatedAt).IsRequired();

        builder.HasIndex(x => new { x.ApplicationUserId, x.AuthorContentId }).IsUnique();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany()
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AuthorContent)
            .WithMany()
            .HasForeignKey(x => x.AuthorContentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

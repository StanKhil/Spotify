using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.User;

namespace Spotify.Infrastructure.Persistance.Context.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasOne(x => x.Profile)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey<UserProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Settings)
            .WithMany()
            .HasForeignKey(x => x.SettingsId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(x => x.Playlists)
            .WithOne(x => x.ApplicationUser)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.Author)
            .WithOne(x => x.User)
            .HasForeignKey<Author>(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
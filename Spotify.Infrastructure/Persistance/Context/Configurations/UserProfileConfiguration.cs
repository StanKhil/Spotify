using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.User;

namespace Spotify.Infrastructure.Persistance.Context.Configurations;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.UserId);

        builder.Property(x => x.Birthdate)
            .IsRequired();

        builder.Property(x => x.RegisteredAt)
            .IsRequired();

        builder.HasOne(x => x.Country)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.CountryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(x => x.City)
            .WithMany(x => x.Users)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
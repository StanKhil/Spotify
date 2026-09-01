using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Security;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class RevokedTokenConfiguration : IEntityTypeConfiguration<RevokedToken>
    {
        public void Configure(EntityTypeBuilder<RevokedToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasIndex(x => x.Jti)
                .IsUnique();

            builder.Property(x => x.Jti)
                .IsRequired();

            builder.Property(x => x.ExpiresAtUtc)
                .IsRequired();

            builder.Property(x => x.RevokedAtUtc)
                .IsRequired();
        }
    }
}
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public sealed class AuthorSubscriptionConfiguration
    : IEntityTypeConfiguration<AuthorSubscription>
    {
        public void Configure(EntityTypeBuilder<AuthorSubscription> builder)
        {
            builder.ToTable("AuthorSubscriptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.HasOne(x => x.ApplicationUser)
                .WithMany(x => x.AuthorSubscriptions)
                .HasForeignKey(x => x.ApplicationUserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Author)
                .WithMany(x => x.Followers)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => new
            {
                x.ApplicationUserId,
                x.AuthorId
            })
            .IsUnique();
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    internal class AuthorContentAuthorConfiguration : IEntityTypeConfiguration<AuthorContentAuthor>
    {
        public void Configure(EntityTypeBuilder<AuthorContentAuthor> builder)
        {
            builder.HasKey(x => new { x.AuthorContentId, x.AuthorId });

            builder.HasOne(x => x.AuthorContent)
                .WithMany(ac => ac.Authors)
                .HasForeignKey(x => x.AuthorContentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Author)
                .WithMany(a => a.AuthoredContent)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.AuthorContentId, x.AuthorId })
                .IsUnique();
        }
    }
}

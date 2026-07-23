using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.AuthorContent)
                .WithMany()
                .HasForeignKey(x => x.AuthorContentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<UserAccess>()
                .WithMany(ua => ua.Likes)
                .HasForeignKey(x => x.UserAccessId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

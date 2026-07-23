using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class AlbumConfiguration : IEntityTypeConfiguration<Album>
    {
        public void Configure(EntityTypeBuilder<Album> builder)
        {

            builder.HasOne(x => x.CoverImage)
                .WithMany(x => x.Albums)
                .HasForeignKey(x => x.CoverImageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

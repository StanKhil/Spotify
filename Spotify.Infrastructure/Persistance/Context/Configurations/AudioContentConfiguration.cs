using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class AudioContentConfiguration : IEntityTypeConfiguration<AudioContent>
    {
        public void Configure(EntityTypeBuilder<AudioContent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Description)
                .HasMaxLength(2000);

            builder.Property(x => x.DurationSeconds)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.IsForAdult)
                .IsRequired();

            builder.HasOne(x => x.ImageItem)
                .WithMany()
                .HasForeignKey(x => x.ImageItemId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.AudioItem)
                .WithMany()
                .HasForeignKey(x => x.AudioItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Genre)
                .WithMany()
                .HasForeignKey(x => x.GenreId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(x => x.LastPlayedEntries)
                .WithOne(x => x.AudioContent)
                .HasForeignKey(x => x.AudioContentId);

            builder.HasMany(x => x.Authors)
                .WithOne(x => x.Item)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}

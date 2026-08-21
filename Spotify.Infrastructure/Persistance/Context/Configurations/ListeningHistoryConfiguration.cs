using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

public sealed class ListeningHistoryConfiguration
    : IEntityTypeConfiguration<ListeningHistory>
{
    public void Configure(EntityTypeBuilder<ListeningHistory> builder)
    {
        builder.ToTable("ListeningHistory");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.ListenedSeconds)
            .IsRequired();

        builder.Property(x => x.IsCompleted)
            .IsRequired();

        builder.Property(x => x.PlayedAt)
            .IsRequired();

        builder.HasOne(x => x.ApplicationUser)
            .WithMany(x => x.ListeningHistory)
            .HasForeignKey(x => x.ApplicationUserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.AuthorContent)
            .WithMany()
            .HasForeignKey(x => x.AuthorContentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => new
        {
            x.ApplicationUserId,
            x.PlayedAt
        });

        
    }
}
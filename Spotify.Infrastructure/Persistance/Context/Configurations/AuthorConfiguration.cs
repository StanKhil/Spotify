using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;

namespace Spotify.Infrastructure.Persistance.Context.Configurations;

public class AuthorConfiguration : IEntityTypeConfiguration<Author>
{
    public void Configure(EntityTypeBuilder<Author> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(x => x.MonthList)
            .HasDefaultValue(0);

        builder.Property(x => x.Bio)
            .HasMaxLength(2000);

        builder.Property(x => x.ExternalAuthorId)
            .HasMaxLength(200);

        builder.HasIndex(x => x.ExternalAuthorId)
            .IsUnique()
            .HasFilter("[ExternalAuthorId] IS NOT NULL");

        builder.HasOne(x => x.User)
            .WithOne(x => x.Author)
            .HasForeignKey<Author>(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(x => x.BioImageItem)
            .WithMany()
            .HasForeignKey(x => x.BioImageItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.AuthoredContent)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Followers)
            .WithOne(x => x.Author)
            .HasForeignKey(x => x.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
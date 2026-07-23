using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class AuthorContentConfiguration : IEntityTypeConfiguration<AuthorContent>
    {
        public void Configure(EntityTypeBuilder<AuthorContent> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Author)
                .WithMany(x => x.AuthoredContent)
                .HasForeignKey(x => x.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Item)
                .WithMany(x => x.Authors)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasIndex(x => new { x.AuthorId, x.ItemId })
                .IsUnique();
        }
    }
}

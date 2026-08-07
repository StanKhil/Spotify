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

            builder.HasOne(x => x.Item)
                .WithMany(x => x.Authors)
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Authors)
                .WithOne(x => x.AuthorContent)
                .HasForeignKey(x => x.AuthorContentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

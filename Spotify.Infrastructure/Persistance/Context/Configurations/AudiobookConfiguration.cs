using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class AudiobookConfiguration : IEntityTypeConfiguration<Audiobook>
    {
        public void Configure(EntityTypeBuilder<Audiobook> builder)
        {
            builder.HasOne(x => x.AuthorContent)
                .WithMany()
                .HasForeignKey(x => x.AuthorContentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

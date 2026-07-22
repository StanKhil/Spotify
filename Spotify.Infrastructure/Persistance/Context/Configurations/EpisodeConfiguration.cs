using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class EpisodeConfiguration : IEntityTypeConfiguration<Episode>
    {
        public void Configure(EntityTypeBuilder<Episode> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Podcast)
                .WithMany(x => x.Episodes)
                .HasForeignKey(x => x.PodcastId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

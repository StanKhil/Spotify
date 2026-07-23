using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Spotify.Domain.Entities.Content;
using System;
using System.Collections.Generic;
using System.Text;

namespace Spotify.Infrastructure.Persistance.Context.Configurations
{
    public class UserAccessConfiguration : IEntityTypeConfiguration<UserAccess>
    {
        public void Configure(EntityTypeBuilder<UserAccess> builder)
        {
            builder.HasKey(ua => ua.Id);

            //builder.Property(x => x.Login)
            //    .IsRequired()
            //    .HasMaxLength(100);
            //
            //builder.Property(x => x.Salt)
            //    .IsRequired();
            //
            //builder.Property(x => x.DerivedKey)
            //    .IsRequired();

            //builder.HasMany(ua => ua.Likes)
            //    .WithOne()
            //    .HasForeignKey(l => l.UserAccessId)
            //    .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ua => ua.User)
                .WithOne(ud => ud.UserAccess)
                .HasForeignKey<UserAccess>(ua => ua.UserId);

            builder.HasMany(ua => ua.Playlists)
                .WithOne(p => p.UserAccess)
                .HasForeignKey(ua => ua.UserAccessId);

            builder.HasMany(ua => ua.AuthoredContent)
                .WithOne(ac => ac.Author)
                .HasForeignKey(ua => ua.AuthorId);
        }
    }
}

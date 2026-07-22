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
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Login)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(x => x.Salt)
                .IsRequired();

            builder.Property(x => x.DerivedKey)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithOne(x => x.UserAccess)
                .HasForeignKey<UserAccess>(x => x.UserId);
        }
    }
}

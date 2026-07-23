using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spotify.Domain.Entities.Content;
using Spotify.Infrastructure.Persistance.Context.Configurations;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Spotify.Infrastructure.Persistance.Context
{
    public class ApplicationContext : IdentityDbContext<UserAccess, UserRole, Guid>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<Album> Albums { get; set; } = null!;
        public DbSet<Audiobook> Audiobooks { get; set; } = null!;
        public DbSet<AudioItem> AudioItems { get; set; } = null!;
        public DbSet<AuthorContent> AuthorContents { get; set; } = null!;
        public DbSet<CoverImage> CoverImages { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Episode> Episodes { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<LastPlayed> LastPlayedEntries { get; set; } = null!;
        public DbSet<ImageItem> ImageItems { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<Mood> Moods { get; set; } = null!;
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<Settings> Settings { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Podcast> Podcasts { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Track> Tracks { get; set; } = null!;
        public DbSet<TrackTag> TrackTags { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<UserAccess> UserAccesses { get; set; } = null!;
        public DbSet<UserData> UserDatas { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserAccess>().ToTable("UserAccesses");
            builder.Entity<UserRole>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");

            builder.Entity<IdentityUserRole<Guid>>().HasOne<UserAccess>().WithMany().HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<IdentityUserRole<Guid>>().HasOne<UserRole>().WithMany().HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Subscription>().HasMany(s => s.UserAccesses).WithOne(ua => ua.Subscription).HasForeignKey(ua => ua.SubscriptionId);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

        }
    }
}

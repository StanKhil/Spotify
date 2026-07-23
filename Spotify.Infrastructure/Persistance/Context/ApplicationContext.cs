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

            builder.ApplyConfiguration(new AlbumConfiguration());
        }
    }
}

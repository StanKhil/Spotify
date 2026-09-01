using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Spotify.Domain.Entities.Content;
using Spotify.Domain.Entities.Location;
using Spotify.Domain.Entities.Security;
using Spotify.Domain.Entities.User;
using Spotify.Domain.Enumerations;

namespace Spotify.Infrastructure.Persistance.Context
{
    public class ApplicationContext : IdentityDbContext<ApplicationUser, UserRole, Guid>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<Album> Albums { get; set; } = null!;
        public DbSet<Audiobook> Audiobooks { get; set; } = null!;
        public DbSet<AudioItem> AudioItems { get; set; } = null!;
        public DbSet<AuthorContent> AuthorContents { get; set; } = null!;
        public DbSet<AuthorContentAuthor> AuthorContentAuthors { get; set; } = null!;
        public DbSet<Country> Countries { get; set; } = null!;
        public DbSet<Episode> Episodes { get; set; } = null!;
        public DbSet<Genre> Genres { get; set; } = null!;
        public DbSet<LastPlayed> LastPlayedEntries { get; set; } = null!;
        public DbSet<ImageItem> ImageItems { get; set; } = null!;
        public DbSet<Like> Likes { get; set; } = null!;
        public DbSet<License> Licenses { get; set; } = null!;
        public DbSet<Mood> Moods { get; set; } = null!;
        public DbSet<Playlist> Playlists { get; set; } = null!;
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; } = null!;
        public DbSet<Settings> Settings { get; set; } = null!;
        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Podcast> Podcasts { get; set; } = null!;
        public DbSet<Tag> Tags { get; set; } = null!;
        public DbSet<Track> Tracks { get; set; } = null!;
        public DbSet<TrackTag> TrackTags { get; set; } = null!;
        public DbSet<City> Cities { get; set; } = null!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<Plugin> Plugins { get; set; } = null!;
        public DbSet<SystemSetting> SystemSettings { get; set; } = null!;
        public DbSet<ListeningHistory> ListeningHistories { get; set; } = null!;
        public DbSet<AuthorSubscription> AuthorSubscriptions { get; set; } = null!;
        public DbSet<RevokedToken> RevokedTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<SystemSetting>().HasKey(x => x.Key);
            builder.Entity<ApplicationUser>().ToTable("ApplicationUsers");
            builder.Entity<UserRole>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaims");
            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRoles");

            builder.Entity<IdentityUserRole<Guid>>().HasOne<ApplicationUser>().WithMany().HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<IdentityUserRole<Guid>>().HasOne<UserRole>().WithMany().HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Subscription>().HasMany(s => s.ApplicationUsers).WithOne(ua => ua.Subscription).HasForeignKey(ua => ua.SubscriptionId);

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);

            SeedRoles(builder);
            SeedSubscriptions(builder);
            SeedUsers(builder);
            SeedCountries(builder);
            SeedCities(builder);
        }

        private static void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Name = "Self-Registered",
                    NormalizedName = "SELF-REGISTERED",
                    Description = "Default role for self-registered users",
                    ConcurrencyStamp = "11111111-1111-1111-1111-111111111111",
                    CanCreate = true,
                    CanRead = true,
                    CanUpdate = false,
                    CanDelete = false,
                },
                new UserRole
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Description = "Administrator role with full permissions",
                    ConcurrencyStamp = "22222222-2222-2222-2222-222222222222",
                    CanCreate = true,
                    CanRead = true,
                    CanUpdate = true,
                    CanDelete = true
                });
        }

        private static void SeedSubscriptions(ModelBuilder builder)
        {

            builder.Entity<Subscription>().HasData(
                new Subscription
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Name = "Default",
                    Description = "Default subscription",
                    Price = 0
                },
                new Subscription
                {
                    Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                    Name = "Premium",
                    Description = "Premium subscription",
                    Price = 9.99m
                },
                new Subscription
                {
                    Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                    Name = "Student",
                    Description = "Student subscription",
                    Price = 4.99m
                });
        }

        private static void SeedUsers(ModelBuilder builder)
        {
            builder.Entity<Settings>().HasData(
                new Settings
                {
                    Id = Guid.Parse(
                        "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    Language = Language.Ukrainian,
                });

            builder.Entity<ApplicationUser>().HasData(
                new ApplicationUser
                {
                    Id = Guid.Parse("66666666-6666-6666-6666-666666666666"),

                    UserName = "admin",
                    NormalizedUserName = "ADMIN",

                    Email = "admin@example.com",
                    NormalizedEmail = "ADMIN@EXAMPLE.COM",

                    EmailConfirmed = true,

                    IsAuthor = true,

                    SubscriptionId = Guid.Parse("44444444-4444-4444-4444-444444444444"),

                    SettingsId = Guid.Parse(
                        "aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
                    ConcurrencyStamp =
                        "66666666-6666-6666-6666-666666666666"
                });

            builder.Entity<UserProfile>().HasData(
                new UserProfile
                {
                    UserId = Guid.Parse("66666666-6666-6666-6666-666666666666"),

                    CountryId = Guid.Parse("1084aaa8-e8c0-42eb-8153-4e0d79955220"),
                    CityId = Guid.Parse("5758bf18-11e6-44a6-ae60-1d8ab273eb49"),

                    Birthdate = new DateTime(
                        1990, 1, 1,
                        0, 0, 0,
                        DateTimeKind.Utc),

                    IsAdult = true,

                    RegisteredAt = new DateTime(
                        2026, 1, 1,
                        0, 0, 0,
                        DateTimeKind.Utc),

                    DeletedAt = null
                });

          
            builder.Entity<IdentityUserRole<Guid>>().HasData(
                new IdentityUserRole<Guid>
                {
                    UserId = Guid.Parse("66666666-6666-6666-6666-666666666666"),
                    RoleId = Guid.Parse("22222222-2222-2222-2222-222222222222")
                });
        }

        private static void SeedCountries(ModelBuilder builder)
        {
            var countries = new[]
            {
                new Country { Id = Guid.Parse("1084aaa8-e8c0-42eb-8153-4e0d79955220"), Name = "United States" },
                new Country { Id = Guid.Parse("2084aaa8-e8c0-42eb-8153-4e0d79955221"), Name = "China" },
                new Country { Id = Guid.Parse("3084aaa8-e8c0-42eb-8153-4e0d79955222"), Name = "India" },
                new Country { Id = Guid.Parse("4084aaa8-e8c0-42eb-8153-4e0d79955223"), Name = "Brazil" },
                new Country { Id = Guid.Parse("5084aaa8-e8c0-42eb-8153-4e0d79955224"), Name = "Germany" },
                new Country { Id = Guid.Parse("6084aaa8-e8c0-42eb-8153-4e0d79955225"), Name = "United Kingdom" },
                new Country { Id = Guid.Parse("7084aaa8-e8c0-42eb-8153-4e0d79955226"), Name = "France" },
                new Country { Id = Guid.Parse("8084aaa8-e8c0-42eb-8153-4e0d79955227"), Name = "Italy" },
                new Country { Id = Guid.Parse("9084aaa8-e8c0-42eb-8153-4e0d79955228"), Name = "Spain" },
                new Country { Id = Guid.Parse("a084aaa8-e8c0-42eb-8153-4e0d79955229"), Name = "Canada" },
                new Country { Id = Guid.Parse("b084aaa8-e8c0-42eb-8153-4e0d7995522a"), Name = "Australia" },
                new Country { Id = Guid.Parse("c084aaa8-e8c0-42eb-8153-4e0d7995522b"), Name = "Japan" },
                new Country { Id = Guid.Parse("d084aaa8-e8c0-42eb-8153-4e0d7995522c"), Name = "South Korea" },
                new Country { Id = Guid.Parse("f084aaa8-e8c0-42eb-8153-4e0d7995522e"), Name = "Mexico" },
                new Country { Id = Guid.Parse("1184aaa8-e8c0-42eb-8153-4e0d7995522f"), Name = "Netherlands" },
                new Country { Id = Guid.Parse("1284aaa8-e8c0-42eb-8153-4e0d79955230"), Name = "Argentina" },
                new Country { Id = Guid.Parse("1384aaa8-e8c0-42eb-8153-4e0d79955231"), Name = "Sweden" },
                new Country { Id = Guid.Parse("1484aaa8-e8c0-42eb-8153-4e0d79955232"), Name = "Switzerland" },
                new Country { Id = Guid.Parse("1584aaa8-e8c0-42eb-8153-4e0d79955233"), Name = "Singapore" }
            };

            builder.Entity<Country>().HasData(countries);
        }

        private static void SeedCities(ModelBuilder builder)
        {
            var cities = new[]
            {
                new City { Id = Guid.Parse("1111111a-1111-1111-1111-111111111111"), Name = "New York", CountryId = Guid.Parse("1084aaa8-e8c0-42eb-8153-4e0d79955220") },
                new City { Id = Guid.Parse("1111111b-1111-1111-1111-111111111111"), Name = "Los Angeles", CountryId = Guid.Parse("1084aaa8-e8c0-42eb-8153-4e0d79955220") },
                new City { Id = Guid.Parse("1111111c-1111-1111-1111-111111111111"), Name = "Chicago", CountryId = Guid.Parse("1084aaa8-e8c0-42eb-8153-4e0d79955220") },
                new City { Id = Guid.Parse("1111111d-1111-1111-1111-111111111111"), Name = "Houston", CountryId = Guid.Parse("1084aaa8-e8c0-42eb-8153-4e0d79955220") },

                new City { Id = Guid.Parse("2222222a-2222-2222-2222-222222222222"), Name = "Beijing", CountryId = Guid.Parse("2084aaa8-e8c0-42eb-8153-4e0d79955221") },
                new City { Id = Guid.Parse("2222222b-2222-2222-2222-222222222222"), Name = "Shanghai", CountryId = Guid.Parse("2084aaa8-e8c0-42eb-8153-4e0d79955221") },
                new City { Id = Guid.Parse("2222222c-2222-2222-2222-222222222222"), Name = "Guangzhou", CountryId = Guid.Parse("2084aaa8-e8c0-42eb-8153-4e0d79955221") },
                new City { Id = Guid.Parse("2222222d-2222-2222-2222-222222222222"), Name = "Shenzhen", CountryId = Guid.Parse("2084aaa8-e8c0-42eb-8153-4e0d79955221") },

                new City { Id = Guid.Parse("3333333a-3333-3333-3333-333333333333"), Name = "Mumbai", CountryId = Guid.Parse("3084aaa8-e8c0-42eb-8153-4e0d79955222") },
                new City { Id = Guid.Parse("3333333b-3333-3333-3333-333333333333"), Name = "Delhi", CountryId = Guid.Parse("3084aaa8-e8c0-42eb-8153-4e0d79955222") },
                new City { Id = Guid.Parse("3333333c-3333-3333-3333-333333333333"), Name = "Bangalore", CountryId = Guid.Parse("3084aaa8-e8c0-42eb-8153-4e0d79955222") },
                new City { Id = Guid.Parse("3333333d-3333-3333-3333-333333333333"), Name = "Hyderabad", CountryId = Guid.Parse("3084aaa8-e8c0-42eb-8153-4e0d79955222") },

                new City { Id = Guid.Parse("4444444a-4444-4444-4444-444444444444"), Name = "São Paulo", CountryId = Guid.Parse("4084aaa8-e8c0-42eb-8153-4e0d79955223") },
                new City { Id = Guid.Parse("4444444b-4444-4444-4444-444444444444"), Name = "Rio de Janeiro", CountryId = Guid.Parse("4084aaa8-e8c0-42eb-8153-4e0d79955223") },
                new City { Id = Guid.Parse("4444444c-4444-4444-4444-444444444444"), Name = "Brasília", CountryId = Guid.Parse("4084aaa8-e8c0-42eb-8153-4e0d79955223") },
                new City { Id = Guid.Parse("4444444d-4444-4444-4444-444444444444"), Name = "Salvador", CountryId = Guid.Parse("4084aaa8-e8c0-42eb-8153-4e0d79955223") },

                new City { Id = Guid.Parse("5555555a-5555-5555-5555-555555555555"), Name = "Berlin", CountryId = Guid.Parse("5084aaa8-e8c0-42eb-8153-4e0d79955224") },
                new City { Id = Guid.Parse("5555555b-5555-5555-5555-555555555555"), Name = "Munich", CountryId = Guid.Parse("5084aaa8-e8c0-42eb-8153-4e0d79955224") },
                new City { Id = Guid.Parse("5555555c-5555-5555-5555-555555555555"), Name = "Hamburg", CountryId = Guid.Parse("5084aaa8-e8c0-42eb-8153-4e0d79955224") },
                new City { Id = Guid.Parse("5555555d-5555-5555-5555-555555555555"), Name = "Frankfurt", CountryId = Guid.Parse("5084aaa8-e8c0-42eb-8153-4e0d79955224") },

                new City { Id = Guid.Parse("6666666a-6666-6666-6666-666666666666"), Name = "London", CountryId = Guid.Parse("6084aaa8-e8c0-42eb-8153-4e0d79955225") },
                new City { Id = Guid.Parse("6666666b-6666-6666-6666-666666666666"), Name = "Manchester", CountryId = Guid.Parse("6084aaa8-e8c0-42eb-8153-4e0d79955225") },
                new City { Id = Guid.Parse("6666666c-6666-6666-6666-666666666666"), Name = "Birmingham", CountryId = Guid.Parse("6084aaa8-e8c0-42eb-8153-4e0d79955225") },
                new City { Id = Guid.Parse("6666666d-6666-6666-6666-666666666666"), Name = "Leeds", CountryId = Guid.Parse("6084aaa8-e8c0-42eb-8153-4e0d79955225") },

                new City { Id = Guid.Parse("7777777a-7777-7777-7777-777777777777"), Name = "Paris", CountryId = Guid.Parse("7084aaa8-e8c0-42eb-8153-4e0d79955226") },
                new City { Id = Guid.Parse("7777777b-7777-7777-7777-777777777777"), Name = "Lyon", CountryId = Guid.Parse("7084aaa8-e8c0-42eb-8153-4e0d79955226") },
                new City { Id = Guid.Parse("7777777c-7777-7777-7777-777777777777"), Name = "Marseille", CountryId = Guid.Parse("7084aaa8-e8c0-42eb-8153-4e0d79955226") },
                new City { Id = Guid.Parse("7777777d-7777-7777-7777-777777777777"), Name = "Toulouse", CountryId = Guid.Parse("7084aaa8-e8c0-42eb-8153-4e0d79955226") },

                new City { Id = Guid.Parse("8888888a-8888-8888-8888-888888888888"), Name = "Rome", CountryId = Guid.Parse("8084aaa8-e8c0-42eb-8153-4e0d79955227") },
                new City { Id = Guid.Parse("8888888b-8888-8888-8888-888888888888"), Name = "Milan", CountryId = Guid.Parse("8084aaa8-e8c0-42eb-8153-4e0d79955227") },
                new City { Id = Guid.Parse("8888888c-8888-8888-8888-888888888888"), Name = "Naples", CountryId = Guid.Parse("8084aaa8-e8c0-42eb-8153-4e0d79955227") },
                new City { Id = Guid.Parse("8888888d-8888-8888-8888-888888888888"), Name = "Florence", CountryId = Guid.Parse("8084aaa8-e8c0-42eb-8153-4e0d79955227") },

                new City { Id = Guid.Parse("9999999a-9999-9999-9999-999999999999"), Name = "Madrid", CountryId = Guid.Parse("9084aaa8-e8c0-42eb-8153-4e0d79955228") },
                new City { Id = Guid.Parse("9999999b-9999-9999-9999-999999999999"), Name = "Barcelona", CountryId = Guid.Parse("9084aaa8-e8c0-42eb-8153-4e0d79955228") },
                new City { Id = Guid.Parse("9999999c-9999-9999-9999-999999999999"), Name = "Valencia", CountryId = Guid.Parse("9084aaa8-e8c0-42eb-8153-4e0d79955228") },
                new City { Id = Guid.Parse("9999999d-9999-9999-9999-999999999999"), Name = "Seville", CountryId = Guid.Parse("9084aaa8-e8c0-42eb-8153-4e0d79955228") },

                new City { Id = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Toronto", CountryId = Guid.Parse("a084aaa8-e8c0-42eb-8153-4e0d79955229") },
                new City { Id = Guid.Parse("aaaaaaab-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Vancouver", CountryId = Guid.Parse("a084aaa8-e8c0-42eb-8153-4e0d79955229") },
                new City { Id = Guid.Parse("aaaaaaac-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Montreal", CountryId = Guid.Parse("a084aaa8-e8c0-42eb-8153-4e0d79955229") },
                new City { Id = Guid.Parse("aaaaaaad-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), Name = "Calgary", CountryId = Guid.Parse("a084aaa8-e8c0-42eb-8153-4e0d79955229") },

                new City { Id = Guid.Parse("bbbbbbba-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Sydney", CountryId = Guid.Parse("b084aaa8-e8c0-42eb-8153-4e0d7995522a") },
                new City { Id = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Melbourne", CountryId = Guid.Parse("b084aaa8-e8c0-42eb-8153-4e0d7995522a") },
                new City { Id = Guid.Parse("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Brisbane", CountryId = Guid.Parse("b084aaa8-e8c0-42eb-8153-4e0d7995522a") },
                new City { Id = Guid.Parse("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), Name = "Perth", CountryId = Guid.Parse("b084aaa8-e8c0-42eb-8153-4e0d7995522a") },

                new City { Id = Guid.Parse("ccccccca-cccc-cccc-cccc-cccccccccccc"), Name = "Tokyo", CountryId = Guid.Parse("c084aaa8-e8c0-42eb-8153-4e0d7995522b") },
                new City { Id = Guid.Parse("cccccccb-cccc-cccc-cccc-cccccccccccc"), Name = "Osaka", CountryId = Guid.Parse("c084aaa8-e8c0-42eb-8153-4e0d7995522b") },
                new City { Id = Guid.Parse("cccccccc-cccc-cccc-cccc-cccccccccccc"), Name = "Kyoto", CountryId = Guid.Parse("c084aaa8-e8c0-42eb-8153-4e0d7995522b") },
                new City { Id = Guid.Parse("ccccccc1-cccc-cccc-cccc-cccccccccccc"), Name = "Yokohama", CountryId = Guid.Parse("c084aaa8-e8c0-42eb-8153-4e0d7995522b") },

                new City { Id = Guid.Parse("ddddddda-dddd-dddd-dddd-dddddddddddd"), Name = "Seoul", CountryId = Guid.Parse("d084aaa8-e8c0-42eb-8153-4e0d7995522c") },
                new City { Id = Guid.Parse("dddddddb-dddd-dddd-dddd-dddddddddddd"), Name = "Busan", CountryId = Guid.Parse("d084aaa8-e8c0-42eb-8153-4e0d7995522c") },
                new City { Id = Guid.Parse("dddddddc-dddd-dddd-dddd-dddddddddddd"), Name = "Incheon", CountryId = Guid.Parse("d084aaa8-e8c0-42eb-8153-4e0d7995522c") },
                new City { Id = Guid.Parse("ddddddd1-dddd-dddd-dddd-dddddddddddd"), Name = "Daegu", CountryId = Guid.Parse("d084aaa8-e8c0-42eb-8153-4e0d7995522c") },

                new City { Id = Guid.Parse("ffffffff-0000-0000-0000-000000000001"), Name = "Mexico City", CountryId = Guid.Parse("f084aaa8-e8c0-42eb-8153-4e0d7995522e") },
                new City { Id = Guid.Parse("ffffffff-0000-0000-0000-000000000002"), Name = "Guadalajara", CountryId = Guid.Parse("f084aaa8-e8c0-42eb-8153-4e0d7995522e") },
                new City { Id = Guid.Parse("ffffffff-0000-0000-0000-000000000003"), Name = "Monterrey", CountryId = Guid.Parse("f084aaa8-e8c0-42eb-8153-4e0d7995522e") },
                new City { Id = Guid.Parse("ffffffff-0000-0000-0000-000000000004"), Name = "Cancún", CountryId = Guid.Parse("f084aaa8-e8c0-42eb-8153-4e0d7995522e") },

                new City { Id = Guid.Parse("1111111e-1111-1111-1111-111111111111"), Name = "Amsterdam", CountryId = Guid.Parse("1184aaa8-e8c0-42eb-8153-4e0d7995522f") },
                new City { Id = Guid.Parse("1111111f-1111-1111-1111-111111111111"), Name = "Rotterdam", CountryId = Guid.Parse("1184aaa8-e8c0-42eb-8153-4e0d7995522f") },

                new City { Id = Guid.Parse("1211111e-1111-1111-1111-111111111111"), Name = "Buenos Aires", CountryId = Guid.Parse("1284aaa8-e8c0-42eb-8153-4e0d79955230") },
                new City { Id = Guid.Parse("1211111f-1111-1111-1111-111111111111"), Name = "Córdoba", CountryId = Guid.Parse("1284aaa8-e8c0-42eb-8153-4e0d79955230") },

                new City { Id = Guid.Parse("1311111e-1111-1111-1111-111111111111"), Name = "Stockholm", CountryId = Guid.Parse("1384aaa8-e8c0-42eb-8153-4e0d79955231") },
                new City { Id = Guid.Parse("1311111f-1111-1111-1111-111111111111"), Name = "Gothenburg", CountryId = Guid.Parse("1384aaa8-e8c0-42eb-8153-4e0d79955231") },

                new City { Id = Guid.Parse("1411111e-1111-1111-1111-111111111111"), Name = "Zurich", CountryId = Guid.Parse("1484aaa8-e8c0-42eb-8153-4e0d79955232") },
                new City { Id = Guid.Parse("1411111f-1111-1111-1111-111111111111"), Name = "Geneva", CountryId = Guid.Parse("1484aaa8-e8c0-42eb-8153-4e0d79955232") },

                new City { Id = Guid.Parse("1511111e-1111-1111-1111-111111111111"), Name = "Singapore", CountryId = Guid.Parse("1584aaa8-e8c0-42eb-8153-4e0d79955233") },
                new City { Id = Guid.Parse("5758bf18-11e6-44a6-ae60-1d8ab273eb49"), Name = "Marina Bay", CountryId = Guid.Parse("1584aaa8-e8c0-42eb-8153-4e0d79955233") }
            };

            builder.Entity<City>().HasData(cities);
        }
    }
}
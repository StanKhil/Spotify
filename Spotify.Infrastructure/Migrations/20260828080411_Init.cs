using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AudioItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Provider = table.Column<int>(type: "int", nullable: false),
                    StorageKey = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    ExternalContentId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    BitrateKbps = table.Column<int>(type: "int", nullable: true),
                    LicenseUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: true),
                    IsDownloadAllowed = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoverImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageList = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoverImages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImageItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageList = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Licenses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActivationKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Licenses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Plugins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    SettingsJson = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Plugins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Podcasts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Podcasts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CanCreate = table.Column<bool>(type: "bit", nullable: false),
                    CanRead = table.Column<bool>(type: "bit", nullable: false),
                    CanUpdate = table.Column<bool>(type: "bit", nullable: false),
                    CanDelete = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Language = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(13,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Key = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "Tags",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tags", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Moods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MoodImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Moods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Moods_ImageItems_MoodImageId",
                        column: x => x.MoodImageId,
                        principalTable: "ImageItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApplicationUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsAuthor = table.Column<bool>(type: "bit", nullable: false),
                    SettingsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUsers_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ApplicationUsers_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorSubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorSubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorSubscriptions_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthorSubscriptions_ApplicationUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlists_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserClaims_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAdult = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_UserProfiles_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserProfiles_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRoles_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AudioContent",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsForAdult = table.Column<bool>(type: "bit", nullable: false),
                    ImageItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AudioItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    GenreId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    PodcastId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CoverImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDraft = table.Column<bool>(type: "bit", nullable: true),
                    AuthorContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AlbumId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    MoodId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PlaysNumber = table.Column<long>(type: "bigint", nullable: true),
                    IsAdult = table.Column<bool>(type: "bit", nullable: true),
                    Track_IsDraft = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AudioContent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AudioContent_AudioContent_AlbumId",
                        column: x => x.AlbumId,
                        principalTable: "AudioContent",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AudioContent_AudioItems_AudioItemId",
                        column: x => x.AudioItemId,
                        principalTable: "AudioItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AudioContent_CoverImages_CoverImageId",
                        column: x => x.CoverImageId,
                        principalTable: "CoverImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AudioContent_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AudioContent_ImageItems_ImageItemId",
                        column: x => x.ImageItemId,
                        principalTable: "ImageItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AudioContent_Moods_MoodId",
                        column: x => x.MoodId,
                        principalTable: "Moods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_AudioContent_Podcasts_PodcastId",
                        column: x => x.PodcastId,
                        principalTable: "Podcasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorContents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorContents_AudioContent_ItemId",
                        column: x => x.ItemId,
                        principalTable: "AudioContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistTracks",
                columns: table => new
                {
                    PlaylistId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false),
                    AddedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistTracks", x => new { x.PlaylistId, x.TrackId });
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_AudioContent_TrackId",
                        column: x => x.TrackId,
                        principalTable: "AudioContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PlaylistTracks_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TrackTags",
                columns: table => new
                {
                    TrackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TagId = table.Column<string>(type: "nvarchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackTags", x => new { x.TrackId, x.TagId });
                    table.ForeignKey(
                        name: "FK_TrackTags_AudioContent_TrackId",
                        column: x => x.TrackId,
                        principalTable: "AudioContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TrackTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuthorContentAuthors",
                columns: table => new
                {
                    AuthorContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorContentAuthors", x => new { x.AuthorContentId, x.AuthorId });
                    table.ForeignKey(
                        name: "FK_AuthorContentAuthors_ApplicationUsers_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AuthorContentAuthors_AuthorContents_AuthorContentId",
                        column: x => x.AuthorContentId,
                        principalTable: "AuthorContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LastPlayedEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionSeconds = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastPlayedEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastPlayedEntries_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LastPlayedEntries_AuthorContents_AuthorContentId",
                        column: x => x.AuthorContentId,
                        principalTable: "AuthorContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Likes_AuthorContents_AuthorContentId",
                        column: x => x.AuthorContentId,
                        principalTable: "AuthorContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ListeningHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApplicationUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ListenedSeconds = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ListeningHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ListeningHistory_ApplicationUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ListeningHistory_AuthorContents_AuthorContentId",
                        column: x => x.AuthorContentId,
                        principalTable: "AuthorContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("1084aaa8-e8c0-42eb-8153-4e0d79955220"), "United States" },
                    { new Guid("1184aaa8-e8c0-42eb-8153-4e0d7995522f"), "Netherlands" },
                    { new Guid("1284aaa8-e8c0-42eb-8153-4e0d79955230"), "Argentina" },
                    { new Guid("1384aaa8-e8c0-42eb-8153-4e0d79955231"), "Sweden" },
                    { new Guid("1484aaa8-e8c0-42eb-8153-4e0d79955232"), "Switzerland" },
                    { new Guid("1584aaa8-e8c0-42eb-8153-4e0d79955233"), "Singapore" },
                    { new Guid("2084aaa8-e8c0-42eb-8153-4e0d79955221"), "China" },
                    { new Guid("3084aaa8-e8c0-42eb-8153-4e0d79955222"), "India" },
                    { new Guid("4084aaa8-e8c0-42eb-8153-4e0d79955223"), "Brazil" },
                    { new Guid("5084aaa8-e8c0-42eb-8153-4e0d79955224"), "Germany" },
                    { new Guid("6084aaa8-e8c0-42eb-8153-4e0d79955225"), "United Kingdom" },
                    { new Guid("7084aaa8-e8c0-42eb-8153-4e0d79955226"), "France" },
                    { new Guid("8084aaa8-e8c0-42eb-8153-4e0d79955227"), "Italy" },
                    { new Guid("9084aaa8-e8c0-42eb-8153-4e0d79955228"), "Spain" },
                    { new Guid("a084aaa8-e8c0-42eb-8153-4e0d79955229"), "Canada" },
                    { new Guid("b084aaa8-e8c0-42eb-8153-4e0d7995522a"), "Australia" },
                    { new Guid("c084aaa8-e8c0-42eb-8153-4e0d7995522b"), "Japan" },
                    { new Guid("d084aaa8-e8c0-42eb-8153-4e0d7995522c"), "South Korea" },
                    { new Guid("f084aaa8-e8c0-42eb-8153-4e0d7995522e"), "Mexico" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "CanCreate", "CanDelete", "CanRead", "CanUpdate", "ConcurrencyStamp", "Description", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), true, false, true, false, "11111111-1111-1111-1111-111111111111", "Default role for self-registered users", "Self-Registered", "SELF-REGISTERED" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), true, true, true, true, "22222222-2222-2222-2222-222222222222", "Administrator role with full permissions", "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "Settings",
                columns: new[] { "Id", "Language" },
                values: new object[] { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), 2 });

            migrationBuilder.InsertData(
                table: "Subscriptions",
                columns: new[] { "Id", "Description", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "Default subscription", "Default", 0m },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "Premium subscription", "Premium", 9.99m },
                    { new Guid("55555555-5555-5555-5555-555555555555"), "Student subscription", "Student", 4.99m }
                });

            migrationBuilder.InsertData(
                table: "ApplicationUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "IsAuthor", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "SettingsId", "SubscriptionId", "TwoFactorEnabled", "UserName" },
                values: new object[] { new Guid("66666666-6666-6666-6666-666666666666"), 0, "66666666-6666-6666-6666-666666666666", "admin@example.com", true, true, false, null, "ADMIN@EXAMPLE.COM", "ADMIN", null, null, false, null, new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("44444444-4444-4444-4444-444444444444"), false, "admin" });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "CountryId", "Name" },
                values: new object[,]
                {
                    { new Guid("1111111a-1111-1111-1111-111111111111"), new Guid("1084aaa8-e8c0-42eb-8153-4e0d79955220"), "New York" },
                    { new Guid("1111111b-1111-1111-1111-111111111111"), new Guid("1084aaa8-e8c0-42eb-8153-4e0d79955220"), "Los Angeles" },
                    { new Guid("1111111c-1111-1111-1111-111111111111"), new Guid("1084aaa8-e8c0-42eb-8153-4e0d79955220"), "Chicago" },
                    { new Guid("1111111d-1111-1111-1111-111111111111"), new Guid("1084aaa8-e8c0-42eb-8153-4e0d79955220"), "Houston" },
                    { new Guid("1111111e-1111-1111-1111-111111111111"), new Guid("1184aaa8-e8c0-42eb-8153-4e0d7995522f"), "Amsterdam" },
                    { new Guid("1111111f-1111-1111-1111-111111111111"), new Guid("1184aaa8-e8c0-42eb-8153-4e0d7995522f"), "Rotterdam" },
                    { new Guid("1211111e-1111-1111-1111-111111111111"), new Guid("1284aaa8-e8c0-42eb-8153-4e0d79955230"), "Buenos Aires" },
                    { new Guid("1211111f-1111-1111-1111-111111111111"), new Guid("1284aaa8-e8c0-42eb-8153-4e0d79955230"), "Córdoba" },
                    { new Guid("1311111e-1111-1111-1111-111111111111"), new Guid("1384aaa8-e8c0-42eb-8153-4e0d79955231"), "Stockholm" },
                    { new Guid("1311111f-1111-1111-1111-111111111111"), new Guid("1384aaa8-e8c0-42eb-8153-4e0d79955231"), "Gothenburg" },
                    { new Guid("1411111e-1111-1111-1111-111111111111"), new Guid("1484aaa8-e8c0-42eb-8153-4e0d79955232"), "Zurich" },
                    { new Guid("1411111f-1111-1111-1111-111111111111"), new Guid("1484aaa8-e8c0-42eb-8153-4e0d79955232"), "Geneva" },
                    { new Guid("1511111e-1111-1111-1111-111111111111"), new Guid("1584aaa8-e8c0-42eb-8153-4e0d79955233"), "Singapore" },
                    { new Guid("2222222a-2222-2222-2222-222222222222"), new Guid("2084aaa8-e8c0-42eb-8153-4e0d79955221"), "Beijing" },
                    { new Guid("2222222b-2222-2222-2222-222222222222"), new Guid("2084aaa8-e8c0-42eb-8153-4e0d79955221"), "Shanghai" },
                    { new Guid("2222222c-2222-2222-2222-222222222222"), new Guid("2084aaa8-e8c0-42eb-8153-4e0d79955221"), "Guangzhou" },
                    { new Guid("2222222d-2222-2222-2222-222222222222"), new Guid("2084aaa8-e8c0-42eb-8153-4e0d79955221"), "Shenzhen" },
                    { new Guid("3333333a-3333-3333-3333-333333333333"), new Guid("3084aaa8-e8c0-42eb-8153-4e0d79955222"), "Mumbai" },
                    { new Guid("3333333b-3333-3333-3333-333333333333"), new Guid("3084aaa8-e8c0-42eb-8153-4e0d79955222"), "Delhi" },
                    { new Guid("3333333c-3333-3333-3333-333333333333"), new Guid("3084aaa8-e8c0-42eb-8153-4e0d79955222"), "Bangalore" },
                    { new Guid("3333333d-3333-3333-3333-333333333333"), new Guid("3084aaa8-e8c0-42eb-8153-4e0d79955222"), "Hyderabad" },
                    { new Guid("4444444a-4444-4444-4444-444444444444"), new Guid("4084aaa8-e8c0-42eb-8153-4e0d79955223"), "São Paulo" },
                    { new Guid("4444444b-4444-4444-4444-444444444444"), new Guid("4084aaa8-e8c0-42eb-8153-4e0d79955223"), "Rio de Janeiro" },
                    { new Guid("4444444c-4444-4444-4444-444444444444"), new Guid("4084aaa8-e8c0-42eb-8153-4e0d79955223"), "Brasília" },
                    { new Guid("4444444d-4444-4444-4444-444444444444"), new Guid("4084aaa8-e8c0-42eb-8153-4e0d79955223"), "Salvador" },
                    { new Guid("5555555a-5555-5555-5555-555555555555"), new Guid("5084aaa8-e8c0-42eb-8153-4e0d79955224"), "Berlin" },
                    { new Guid("5555555b-5555-5555-5555-555555555555"), new Guid("5084aaa8-e8c0-42eb-8153-4e0d79955224"), "Munich" },
                    { new Guid("5555555c-5555-5555-5555-555555555555"), new Guid("5084aaa8-e8c0-42eb-8153-4e0d79955224"), "Hamburg" },
                    { new Guid("5555555d-5555-5555-5555-555555555555"), new Guid("5084aaa8-e8c0-42eb-8153-4e0d79955224"), "Frankfurt" },
                    { new Guid("5758bf18-11e6-44a6-ae60-1d8ab273eb49"), new Guid("1584aaa8-e8c0-42eb-8153-4e0d79955233"), "Marina Bay" },
                    { new Guid("6666666a-6666-6666-6666-666666666666"), new Guid("6084aaa8-e8c0-42eb-8153-4e0d79955225"), "London" },
                    { new Guid("6666666b-6666-6666-6666-666666666666"), new Guid("6084aaa8-e8c0-42eb-8153-4e0d79955225"), "Manchester" },
                    { new Guid("6666666c-6666-6666-6666-666666666666"), new Guid("6084aaa8-e8c0-42eb-8153-4e0d79955225"), "Birmingham" },
                    { new Guid("6666666d-6666-6666-6666-666666666666"), new Guid("6084aaa8-e8c0-42eb-8153-4e0d79955225"), "Leeds" },
                    { new Guid("7777777a-7777-7777-7777-777777777777"), new Guid("7084aaa8-e8c0-42eb-8153-4e0d79955226"), "Paris" },
                    { new Guid("7777777b-7777-7777-7777-777777777777"), new Guid("7084aaa8-e8c0-42eb-8153-4e0d79955226"), "Lyon" },
                    { new Guid("7777777c-7777-7777-7777-777777777777"), new Guid("7084aaa8-e8c0-42eb-8153-4e0d79955226"), "Marseille" },
                    { new Guid("7777777d-7777-7777-7777-777777777777"), new Guid("7084aaa8-e8c0-42eb-8153-4e0d79955226"), "Toulouse" },
                    { new Guid("8888888a-8888-8888-8888-888888888888"), new Guid("8084aaa8-e8c0-42eb-8153-4e0d79955227"), "Rome" },
                    { new Guid("8888888b-8888-8888-8888-888888888888"), new Guid("8084aaa8-e8c0-42eb-8153-4e0d79955227"), "Milan" },
                    { new Guid("8888888c-8888-8888-8888-888888888888"), new Guid("8084aaa8-e8c0-42eb-8153-4e0d79955227"), "Naples" },
                    { new Guid("8888888d-8888-8888-8888-888888888888"), new Guid("8084aaa8-e8c0-42eb-8153-4e0d79955227"), "Florence" },
                    { new Guid("9999999a-9999-9999-9999-999999999999"), new Guid("9084aaa8-e8c0-42eb-8153-4e0d79955228"), "Madrid" },
                    { new Guid("9999999b-9999-9999-9999-999999999999"), new Guid("9084aaa8-e8c0-42eb-8153-4e0d79955228"), "Barcelona" },
                    { new Guid("9999999c-9999-9999-9999-999999999999"), new Guid("9084aaa8-e8c0-42eb-8153-4e0d79955228"), "Valencia" },
                    { new Guid("9999999d-9999-9999-9999-999999999999"), new Guid("9084aaa8-e8c0-42eb-8153-4e0d79955228"), "Seville" },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("a084aaa8-e8c0-42eb-8153-4e0d79955229"), "Toronto" },
                    { new Guid("aaaaaaab-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("a084aaa8-e8c0-42eb-8153-4e0d79955229"), "Vancouver" },
                    { new Guid("aaaaaaac-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("a084aaa8-e8c0-42eb-8153-4e0d79955229"), "Montreal" },
                    { new Guid("aaaaaaad-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), new Guid("a084aaa8-e8c0-42eb-8153-4e0d79955229"), "Calgary" },
                    { new Guid("bbbbbbb1-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("b084aaa8-e8c0-42eb-8153-4e0d7995522a"), "Brisbane" },
                    { new Guid("bbbbbbb2-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("b084aaa8-e8c0-42eb-8153-4e0d7995522a"), "Perth" },
                    { new Guid("bbbbbbba-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("b084aaa8-e8c0-42eb-8153-4e0d7995522a"), "Sydney" },
                    { new Guid("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"), new Guid("b084aaa8-e8c0-42eb-8153-4e0d7995522a"), "Melbourne" },
                    { new Guid("ccccccc1-cccc-cccc-cccc-cccccccccccc"), new Guid("c084aaa8-e8c0-42eb-8153-4e0d7995522b"), "Yokohama" },
                    { new Guid("ccccccca-cccc-cccc-cccc-cccccccccccc"), new Guid("c084aaa8-e8c0-42eb-8153-4e0d7995522b"), "Tokyo" },
                    { new Guid("cccccccb-cccc-cccc-cccc-cccccccccccc"), new Guid("c084aaa8-e8c0-42eb-8153-4e0d7995522b"), "Osaka" },
                    { new Guid("cccccccc-cccc-cccc-cccc-cccccccccccc"), new Guid("c084aaa8-e8c0-42eb-8153-4e0d7995522b"), "Kyoto" },
                    { new Guid("ddddddd1-dddd-dddd-dddd-dddddddddddd"), new Guid("d084aaa8-e8c0-42eb-8153-4e0d7995522c"), "Daegu" },
                    { new Guid("ddddddda-dddd-dddd-dddd-dddddddddddd"), new Guid("d084aaa8-e8c0-42eb-8153-4e0d7995522c"), "Seoul" },
                    { new Guid("dddddddb-dddd-dddd-dddd-dddddddddddd"), new Guid("d084aaa8-e8c0-42eb-8153-4e0d7995522c"), "Busan" },
                    { new Guid("dddddddc-dddd-dddd-dddd-dddddddddddd"), new Guid("d084aaa8-e8c0-42eb-8153-4e0d7995522c"), "Incheon" },
                    { new Guid("ffffffff-0000-0000-0000-000000000001"), new Guid("f084aaa8-e8c0-42eb-8153-4e0d7995522e"), "Mexico City" },
                    { new Guid("ffffffff-0000-0000-0000-000000000002"), new Guid("f084aaa8-e8c0-42eb-8153-4e0d7995522e"), "Guadalajara" },
                    { new Guid("ffffffff-0000-0000-0000-000000000003"), new Guid("f084aaa8-e8c0-42eb-8153-4e0d7995522e"), "Monterrey" },
                    { new Guid("ffffffff-0000-0000-0000-000000000004"), new Guid("f084aaa8-e8c0-42eb-8153-4e0d7995522e"), "Cancún" }
                });

            migrationBuilder.InsertData(
                table: "UserProfiles",
                columns: new[] { "UserId", "Birthdate", "CityId", "CountryId", "DeletedAt", "IsAdult", "RegisteredAt" },
                values: new object[] { new Guid("66666666-6666-6666-6666-666666666666"), new DateTime(1990, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new Guid("5758bf18-11e6-44a6-ae60-1d8ab273eb49"), new Guid("1084aaa8-e8c0-42eb-8153-4e0d79955220"), null, true, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "ApplicationUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_SettingsId",
                table: "ApplicationUsers",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUsers_SubscriptionId",
                table: "ApplicationUsers",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "ApplicationUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_AlbumId",
                table: "AudioContent",
                column: "AlbumId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_AudioItemId",
                table: "AudioContent",
                column: "AudioItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_AuthorContentId",
                table: "AudioContent",
                column: "AuthorContentId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_CoverImageId",
                table: "AudioContent",
                column: "CoverImageId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_GenreId",
                table: "AudioContent",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_ImageItemId",
                table: "AudioContent",
                column: "ImageItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_MoodId",
                table: "AudioContent",
                column: "MoodId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_PodcastId",
                table: "AudioContent",
                column: "PodcastId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioItems_Provider_ExternalContentId",
                table: "AudioItems",
                columns: new[] { "Provider", "ExternalContentId" },
                unique: true,
                filter: "[ExternalContentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContentAuthors_AuthorContentId_AuthorId",
                table: "AuthorContentAuthors",
                columns: new[] { "AuthorContentId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContentAuthors_AuthorId",
                table: "AuthorContentAuthors",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContents_ItemId",
                table: "AuthorContents",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorSubscriptions_ApplicationUserId_AuthorId",
                table: "AuthorSubscriptions",
                columns: new[] { "ApplicationUserId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorSubscriptions_AuthorId",
                table: "AuthorSubscriptions",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_LastPlayedEntries_ApplicationUserId_AuthorContentId",
                table: "LastPlayedEntries",
                columns: new[] { "ApplicationUserId", "AuthorContentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LastPlayedEntries_AuthorContentId",
                table: "LastPlayedEntries",
                column: "AuthorContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_ApplicationUserId",
                table: "Likes",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorContentId",
                table: "Likes",
                column: "AuthorContentId");

            migrationBuilder.CreateIndex(
                name: "IX_ListeningHistory_ApplicationUserId_PlayedAt",
                table: "ListeningHistory",
                columns: new[] { "ApplicationUserId", "PlayedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ListeningHistory_AuthorContentId",
                table: "ListeningHistory",
                column: "AuthorContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Moods_MoodImageId",
                table: "Moods",
                column: "MoodImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_PlaylistId_Position",
                table: "PlaylistTracks",
                columns: new[] { "PlaylistId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_TrackId",
                table: "PlaylistTracks",
                column: "TrackId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "Roles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_TrackTags_TagId",
                table: "TrackTags",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CityId",
                table: "UserProfiles",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CountryId",
                table: "UserProfiles",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioContent_AuthorContents_AuthorContentId",
                table: "AudioContent",
                column: "AuthorContentId",
                principalTable: "AuthorContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioContent_AudioItems_AudioItemId",
                table: "AudioContent");

            migrationBuilder.DropForeignKey(
                name: "FK_AudioContent_AuthorContents_AuthorContentId",
                table: "AudioContent");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "AuthorContentAuthors");

            migrationBuilder.DropTable(
                name: "AuthorSubscriptions");

            migrationBuilder.DropTable(
                name: "LastPlayedEntries");

            migrationBuilder.DropTable(
                name: "Licenses");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "ListeningHistory");

            migrationBuilder.DropTable(
                name: "PlaylistTracks");

            migrationBuilder.DropTable(
                name: "Plugins");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "TrackTags");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "ApplicationUsers");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "AudioItems");

            migrationBuilder.DropTable(
                name: "AuthorContents");

            migrationBuilder.DropTable(
                name: "AudioContent");

            migrationBuilder.DropTable(
                name: "CoverImages");

            migrationBuilder.DropTable(
                name: "Genres");

            migrationBuilder.DropTable(
                name: "Moods");

            migrationBuilder.DropTable(
                name: "Podcasts");

            migrationBuilder.DropTable(
                name: "ImageItems");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AudioItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudioList = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                name: "UserDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsAdult = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDatas_Cities_CityId",
                        column: x => x.CityId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserDatas_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubscriptionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_UserAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAccesses_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccesses_Settings_SettingsId",
                        column: x => x.SettingsId,
                        principalTable: "Settings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccesses_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserAccesses_UserDatas_UserId",
                        column: x => x.UserId,
                        principalTable: "UserDatas",
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
                        name: "FK_AspNetUserLogins_UserAccesses_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccesses",
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
                        name: "FK_AspNetUserTokens_UserAccesses_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Playlists",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    UserAccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Playlists", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Playlists_UserAccesses_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "UserAccesses",
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
                        name: "FK_UserClaims_UserAccesses_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
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
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_UserAccesses_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccesses",
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
                    AudioItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    Track_IsDraft = table.Column<bool>(type: "bit", nullable: true),
                    PlaylistId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                        principalColumn: "Id");
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
                        name: "FK_AudioContent_Playlists_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "Playlists",
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
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthorContents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthorContents_AudioContent_ItemId",
                        column: x => x.ItemId,
                        principalTable: "AudioContent",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AuthorContents_UserAccesses_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "UserAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LastPlayedEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AudioContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastPlayedEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastPlayedEntries_AudioContent_AudioContentId",
                        column: x => x.AudioContentId,
                        principalTable: "AudioContent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LastPlayedEntries_UserAccesses_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "UserAccesses",
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
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorContentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserAccessId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_AuthorContents_AuthorContentId",
                        column: x => x.AuthorContentId,
                        principalTable: "AuthorContents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_UserAccesses_UserAccessId",
                        column: x => x.UserAccessId,
                        principalTable: "UserAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "IX_AudioContent_PlaylistId",
                table: "AudioContent",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_PodcastId",
                table: "AudioContent",
                column: "PodcastId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContents_AuthorId_ItemId",
                table: "AuthorContents",
                columns: new[] { "AuthorId", "ItemId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContents_ItemId",
                table: "AuthorContents",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Cities_CountryId",
                table: "Cities",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_LastPlayedEntries_AudioContentId",
                table: "LastPlayedEntries",
                column: "AudioContentId");

            migrationBuilder.CreateIndex(
                name: "IX_LastPlayedEntries_UserAccessId",
                table: "LastPlayedEntries",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_AuthorContentId",
                table: "Likes",
                column: "AuthorContentId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserAccessId",
                table: "Likes",
                column: "UserAccessId");

            migrationBuilder.CreateIndex(
                name: "IX_Moods_MoodImageId",
                table: "Moods",
                column: "MoodImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_UserAccessId",
                table: "Playlists",
                column: "UserAccessId");

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
                name: "EmailIndex",
                table: "UserAccesses",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_RoleId",
                table: "UserAccesses",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_SettingsId",
                table: "UserAccesses",
                column: "SettingsId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_SubscriptionId",
                table: "UserAccesses",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_UserId",
                table: "UserAccesses",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "UserAccesses",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaims_UserId",
                table: "UserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_CityId",
                table: "UserDatas",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_CountryId",
                table: "UserDatas",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDatas_Email",
                table: "UserDatas",
                column: "Email",
                unique: true);

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
                name: "FK_UserAccesses_Roles_RoleId",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_UserAccesses_AuthorId",
                table: "AuthorContents");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_UserAccesses_UserAccessId",
                table: "Playlists");

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
                name: "LastPlayedEntries");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "TrackTags");

            migrationBuilder.DropTable(
                name: "UserClaims");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "Tags");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "UserAccesses");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "UserDatas");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Countries");

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
                name: "Playlists");

            migrationBuilder.DropTable(
                name: "Podcasts");

            migrationBuilder.DropTable(
                name: "ImageItems");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserConfFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LastPlayedEntries_UserAccesses_UserAccessId",
                table: "LastPlayedEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_UserAccesses_UserAccessId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_UserAccesses_UserAccessId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Roles_RoleId",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_UserDatas_UserId",
                table: "UserAccesses");

            migrationBuilder.DropTable(
                name: "UserDatas");

            migrationBuilder.DropIndex(
                name: "IX_UserAccesses_RoleId",
                table: "UserAccesses");

            migrationBuilder.DropIndex(
                name: "IX_UserAccesses_UserId",
                table: "UserAccesses");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "UserAccesses");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserAccesses");

            migrationBuilder.RenameColumn(
                name: "UserAccessId",
                table: "Playlists",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Playlists_UserAccessId",
                table: "Playlists",
                newName: "IX_Playlists_ApplicationUserId");

            migrationBuilder.RenameColumn(
                name: "UserAccessId",
                table: "Likes",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_UserAccessId",
                table: "Likes",
                newName: "IX_Likes_ApplicationUserId");

            migrationBuilder.RenameColumn(
                name: "UserAccessId",
                table: "LastPlayedEntries",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_LastPlayedEntries_UserAccessId",
                table: "LastPlayedEntries",
                newName: "IX_LastPlayedEntries_ApplicationUserId");

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
                    table.ForeignKey(
                        name: "FK_UserProfiles_UserAccesses_UserId",
                        column: x => x.UserId,
                        principalTable: "UserAccesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CityId",
                table: "UserProfiles",
                column: "CityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CountryId",
                table: "UserProfiles",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_LastPlayedEntries_UserAccesses_ApplicationUserId",
                table: "LastPlayedEntries",
                column: "ApplicationUserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_UserAccesses_ApplicationUserId",
                table: "Likes",
                column: "ApplicationUserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_UserAccesses_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LastPlayedEntries_UserAccesses_ApplicationUserId",
                table: "LastPlayedEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_UserAccesses_ApplicationUserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_UserAccesses_ApplicationUserId",
                table: "Playlists");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Playlists",
                newName: "UserAccessId");

            migrationBuilder.RenameIndex(
                name: "IX_Playlists_ApplicationUserId",
                table: "Playlists",
                newName: "IX_Playlists_UserAccessId");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "Likes",
                newName: "UserAccessId");

            migrationBuilder.RenameIndex(
                name: "IX_Likes_ApplicationUserId",
                table: "Likes",
                newName: "IX_Likes_UserAccessId");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "LastPlayedEntries",
                newName: "UserAccessId");

            migrationBuilder.RenameIndex(
                name: "IX_LastPlayedEntries_ApplicationUserId",
                table: "LastPlayedEntries",
                newName: "IX_LastPlayedEntries_UserAccessId");

            migrationBuilder.AddColumn<Guid>(
                name: "RoleId",
                table: "UserAccesses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "UserAccesses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "UserDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CountryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Birthdate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    IsAdult = table.Column<bool>(type: "bit", nullable: false),
                    RegisteredAt = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_RoleId",
                table: "UserAccesses",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAccesses_UserId",
                table: "UserAccesses",
                column: "UserId",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_LastPlayedEntries_UserAccesses_UserAccessId",
                table: "LastPlayedEntries",
                column: "UserAccessId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_UserAccesses_UserAccessId",
                table: "Likes",
                column: "UserAccessId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_UserAccesses_UserAccessId",
                table: "Playlists",
                column: "UserAccessId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Roles_RoleId",
                table: "UserAccesses",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_UserDatas_UserId",
                table: "UserAccesses",
                column: "UserId",
                principalTable: "UserDatas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

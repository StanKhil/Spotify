using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TrackActions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LastPlayedEntries_AudioContent_AudioContentId",
                table: "LastPlayedEntries");

            migrationBuilder.RenameColumn(
                name: "AudioContentId",
                table: "LastPlayedEntries",
                newName: "AuthorContentId");

            migrationBuilder.RenameIndex(
                name: "IX_LastPlayedEntries_AudioContentId",
                table: "LastPlayedEntries",
                newName: "IX_LastPlayedEntries_AuthorContentId");

            migrationBuilder.RenameIndex(
                name: "IX_LastPlayedEntries_ApplicationUserId_AudioContentId",
                table: "LastPlayedEntries",
                newName: "IX_LastPlayedEntries_ApplicationUserId_AuthorContentId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ListeningHistory_ApplicationUserId_PlayedAt",
                table: "ListeningHistory",
                columns: new[] { "ApplicationUserId", "PlayedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_ListeningHistory_AuthorContentId",
                table: "ListeningHistory",
                column: "AuthorContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LastPlayedEntries_AuthorContents_AuthorContentId",
                table: "LastPlayedEntries",
                column: "AuthorContentId",
                principalTable: "AuthorContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LastPlayedEntries_AuthorContents_AuthorContentId",
                table: "LastPlayedEntries");

            migrationBuilder.DropTable(
                name: "ListeningHistory");

            migrationBuilder.RenameColumn(
                name: "AuthorContentId",
                table: "LastPlayedEntries",
                newName: "AudioContentId");

            migrationBuilder.RenameIndex(
                name: "IX_LastPlayedEntries_AuthorContentId",
                table: "LastPlayedEntries",
                newName: "IX_LastPlayedEntries_AudioContentId");

            migrationBuilder.RenameIndex(
                name: "IX_LastPlayedEntries_ApplicationUserId_AuthorContentId",
                table: "LastPlayedEntries",
                newName: "IX_LastPlayedEntries_ApplicationUserId_AudioContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_LastPlayedEntries_AudioContent_AudioContentId",
                table: "LastPlayedEntries",
                column: "AudioContentId",
                principalTable: "AudioContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

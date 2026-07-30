using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NormalizePlaybackModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioContent_AudioItems_AudioItemId",
                table: "AudioContent");

            migrationBuilder.DropForeignKey(
                name: "FK_AudioContent_Playlists_PlaylistId",
                table: "AudioContent");

            migrationBuilder.DropIndex(
                name: "IX_LastPlayedEntries_ApplicationUserId",
                table: "LastPlayedEntries");

            migrationBuilder.DropIndex(
                name: "IX_AudioContent_PlaylistId",
                table: "AudioContent");

            migrationBuilder.RenameColumn(
                name: "AudioList",
                table: "AudioItems",
                newName: "StorageKey");

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "LastPlayedEntries",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "PositionSeconds",
                table: "LastPlayedEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "LastPlayedEntries",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "BitrateKbps",
                table: "AudioItems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentType",
                table: "AudioItems",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AudioItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ExternalContentId",
                table: "AudioItems",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDownloadAllowed",
                table: "AudioItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "LicenseUrl",
                table: "AudioItems",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "AudioItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "AudioItems",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "AudioItemId",
                table: "AudioContent",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

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

            migrationBuilder.Sql(@"
                INSERT INTO PlaylistTracks (PlaylistId, TrackId, Position, AddedAt)
                SELECT PlaylistId,
                       Id,
                       ROW_NUMBER() OVER (PARTITION BY PlaylistId ORDER BY CreatedAt, Id),
                       CreatedAt
                FROM AudioContent
                WHERE PlaylistId IS NOT NULL AND Discriminator = 'Track';");

            migrationBuilder.DropColumn(
                name: "PlaylistId",
                table: "AudioContent");

            migrationBuilder.CreateIndex(
                name: "IX_LastPlayedEntries_ApplicationUserId_AudioContentId",
                table: "LastPlayedEntries",
                columns: new[] { "ApplicationUserId", "AudioContentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudioItems_Provider_ExternalContentId",
                table: "AudioItems",
                columns: new[] { "Provider", "ExternalContentId" },
                unique: true,
                filter: "[ExternalContentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_PlaylistId_Position",
                table: "PlaylistTracks",
                columns: new[] { "PlaylistId", "Position" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistTracks_TrackId",
                table: "PlaylistTracks",
                column: "TrackId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioContent_AudioItems_AudioItemId",
                table: "AudioContent",
                column: "AudioItemId",
                principalTable: "AudioItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioContent_AudioItems_AudioItemId",
                table: "AudioContent");

            migrationBuilder.DropTable(
                name: "PlaylistTracks");

            migrationBuilder.DropIndex(
                name: "IX_LastPlayedEntries_ApplicationUserId_AudioContentId",
                table: "LastPlayedEntries");

            migrationBuilder.DropIndex(
                name: "IX_AudioItems_Provider_ExternalContentId",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "LastPlayedEntries");

            migrationBuilder.DropColumn(
                name: "PositionSeconds",
                table: "LastPlayedEntries");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "LastPlayedEntries");

            migrationBuilder.DropColumn(
                name: "BitrateKbps",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "ContentType",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "ExternalContentId",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "IsDownloadAllowed",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "LicenseUrl",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "AudioItems");

            migrationBuilder.RenameColumn(
                name: "StorageKey",
                table: "AudioItems",
                newName: "AudioList");

            migrationBuilder.AlterColumn<Guid>(
                name: "AudioItemId",
                table: "AudioContent",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PlaylistId",
                table: "AudioContent",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LastPlayedEntries_ApplicationUserId",
                table: "LastPlayedEntries",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_PlaylistId",
                table: "AudioContent",
                column: "PlaylistId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioContent_AudioItems_AudioItemId",
                table: "AudioContent",
                column: "AudioItemId",
                principalTable: "AudioItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioContent_Playlists_PlaylistId",
                table: "AudioContent",
                column: "PlaylistId",
                principalTable: "Playlists",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

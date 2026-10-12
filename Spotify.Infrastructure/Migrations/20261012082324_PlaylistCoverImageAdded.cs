using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class PlaylistCoverImageAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CoverImageId",
                table: "Playlists",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Playlists_CoverImageId",
                table: "Playlists",
                column: "CoverImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_ImageItems_CoverImageId",
                table: "Playlists",
                column: "CoverImageId",
                principalTable: "ImageItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_ImageItems_CoverImageId",
                table: "Playlists");

            migrationBuilder.DropIndex(
                name: "IX_Playlists_CoverImageId",
                table: "Playlists");

            migrationBuilder.DropColumn(
                name: "CoverImageId",
                table: "Playlists");
        }
    }
}

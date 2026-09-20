using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LikedAndAudioContetnFixed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioContent_AuthorContents_AuthorContentId",
                table: "AudioContent");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents");

            migrationBuilder.DropIndex(
                name: "IX_AuthorContents_ItemId",
                table: "AuthorContents");

            migrationBuilder.DropIndex(
                name: "IX_AudioContent_AuthorContentId",
                table: "AudioContent");

            migrationBuilder.AddColumn<DateTime>(
                name: "LikedAt",
                table: "Likes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContents_ItemId",
                table: "AuthorContents",
                column: "ItemId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents",
                column: "ItemId",
                principalTable: "AudioContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents");

            migrationBuilder.DropIndex(
                name: "IX_AuthorContents_ItemId",
                table: "AuthorContents");

            migrationBuilder.DropColumn(
                name: "LikedAt",
                table: "Likes");

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContents_ItemId",
                table: "AuthorContents",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_AuthorContentId",
                table: "AudioContent",
                column: "AuthorContentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioContent_AuthorContents_AuthorContentId",
                table: "AudioContent",
                column: "AuthorContentId",
                principalTable: "AuthorContents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents",
                column: "ItemId",
                principalTable: "AudioContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

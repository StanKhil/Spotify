using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthroContentRemoveCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents",
                column: "ItemId",
                principalTable: "AudioContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents",
                column: "ItemId",
                principalTable: "AudioContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

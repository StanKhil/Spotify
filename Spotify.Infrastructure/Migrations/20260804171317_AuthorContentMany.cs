using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthorContentMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_ApplicationUsers_AuthorId",
                table: "AuthorContents");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents");

            migrationBuilder.DropIndex(
                name: "IX_AuthorContents_AuthorId_ItemId",
                table: "AuthorContents");

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "AuthorContents");

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

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContentAuthors_AuthorContentId_AuthorId",
                table: "AuthorContentAuthors",
                columns: new[] { "AuthorContentId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContentAuthors_AuthorId",
                table: "AuthorContentAuthors",
                column: "AuthorId");

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

            migrationBuilder.DropTable(
                name: "AuthorContentAuthors");

            migrationBuilder.AddColumn<Guid>(
                name: "AuthorId",
                table: "AuthorContents",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_AuthorContents_AuthorId_ItemId",
                table: "AuthorContents",
                columns: new[] { "AuthorId", "ItemId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContents_ApplicationUsers_AuthorId",
                table: "AuthorContents",
                column: "AuthorId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContents_AudioContent_ItemId",
                table: "AuthorContents",
                column: "ItemId",
                principalTable: "AudioContent",
                principalColumn: "Id");
        }
    }
}

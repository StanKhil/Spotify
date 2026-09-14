using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPodcastAuthorsAndAudiobookEpisodes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AudiobookId",
                table: "AudioContent",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SeqNumber",
                table: "AudioContent",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_AudiobookId",
                table: "AudioContent",
                column: "AudiobookId");

            migrationBuilder.CreateTable(
                name: "PodcastAuthors",
                columns: table => new
                {
                    PodcastId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PodcastAuthors", x => new { x.PodcastId, x.AuthorId });
                    table.ForeignKey(
                        name: "FK_PodcastAuthors_Authors_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Authors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PodcastAuthors_Podcasts_PodcastId",
                        column: x => x.PodcastId,
                        principalTable: "Podcasts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PodcastAuthors_AuthorId",
                table: "PodcastAuthors",
                column: "AuthorId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioContent_AudioContent_AudiobookId",
                table: "AudioContent",
                column: "AudiobookId",
                principalTable: "AudioContent",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioContent_AudioContent_AudiobookId",
                table: "AudioContent");

            migrationBuilder.DropTable(
                name: "PodcastAuthors");

            migrationBuilder.DropIndex(
                name: "IX_AudioContent_AudiobookId",
                table: "AudioContent");

            migrationBuilder.DropColumn(
                name: "SeqNumber",
                table: "AudioContent");

            migrationBuilder.DropColumn(
                name: "AudiobookId",
                table: "AudioContent");
        }
    }
}
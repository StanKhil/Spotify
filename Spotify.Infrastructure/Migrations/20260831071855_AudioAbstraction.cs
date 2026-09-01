using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AudioAbstraction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AudioItems_Provider_ExternalContentId",
                table: "AudioItems");

            migrationBuilder.DropColumn(
                name: "ExternalContentId",
                table: "AudioItems");

            migrationBuilder.AddColumn<string>(
                name: "ExternalContentId",
                table: "AudioContent",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Provider",
                table: "AudioContent",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_Provider_ExternalContentId",
                table: "AudioContent",
                columns: new[] { "Provider", "ExternalContentId" },
                unique: true,
                filter: "[ExternalContentId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AudioContent_Provider_ExternalContentId",
                table: "AudioContent");

            migrationBuilder.DropColumn(
                name: "ExternalContentId",
                table: "AudioContent");

            migrationBuilder.DropColumn(
                name: "Provider",
                table: "AudioContent");

            migrationBuilder.AddColumn<string>(
                name: "ExternalContentId",
                table: "AudioItems",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AudioItems_Provider_ExternalContentId",
                table: "AudioItems",
                columns: new[] { "Provider", "ExternalContentId" },
                unique: true,
                filter: "[ExternalContentId] IS NOT NULL");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCoverImageUseImageItem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AudioContent_CoverImages_CoverImageId",
                table: "AudioContent");

            migrationBuilder.DropTable(
                name: "CoverImages");

            migrationBuilder.DropIndex(
                name: "IX_AudioContent_CoverImageId",
                table: "AudioContent");

            migrationBuilder.DropColumn(
                name: "CoverImageId",
                table: "AudioContent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CoverImageId",
                table: "AudioContent",
                type: "uniqueidentifier",
                nullable: true);

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

            migrationBuilder.CreateIndex(
                name: "IX_AudioContent_CoverImageId",
                table: "AudioContent",
                column: "CoverImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_AudioContent_CoverImages_CoverImageId",
                table: "AudioContent",
                column: "CoverImageId",
                principalTable: "CoverImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

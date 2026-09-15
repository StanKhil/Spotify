using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProfileImagesAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AvatarImageId",
                table: "UserProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CoverImageId",
                table: "UserProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEHYJhMVQ5lPj0H09UYkEhHG+ExHIow/TLO5WuDiRDSTejnAby4pV9RavmImv+8Hufw==");

            migrationBuilder.UpdateData(
                table: "UserProfiles",
                keyColumn: "UserId",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                columns: new[] { "AvatarImageId", "CoverImageId", "Description" },
                values: new object[] { null, null, null });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_AvatarImageId",
                table: "UserProfiles",
                column: "AvatarImageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CoverImageId",
                table: "UserProfiles",
                column: "CoverImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_ImageItems_AvatarImageId",
                table: "UserProfiles",
                column: "AvatarImageId",
                principalTable: "ImageItems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_ImageItems_CoverImageId",
                table: "UserProfiles",
                column: "CoverImageId",
                principalTable: "ImageItems",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_ImageItems_AvatarImageId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_ImageItems_CoverImageId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_AvatarImageId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_CoverImageId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "AvatarImageId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CoverImageId",
                table: "UserProfiles");

            migrationBuilder.UpdateData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "PasswordHash",
                value: null);
        }
    }
}

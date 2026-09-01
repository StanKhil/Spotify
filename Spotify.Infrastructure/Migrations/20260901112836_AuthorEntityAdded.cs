using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AuthorEntityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContentAuthors_ApplicationUsers_AuthorId",
                table: "AuthorContentAuthors");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorSubscriptions_ApplicationUsers_AuthorId",
                table: "AuthorSubscriptions");

            migrationBuilder.DropColumn(
                name: "IsAuthor",
                table: "ApplicationUsers");

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ExternalAuthorId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Authors_ApplicationUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "ApplicationUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "ExternalAuthorId", "Name", "UserId" },
                values: new object[] { new Guid("77777777-7777-7777-7777-777777777777"), null, "Admin Author", new Guid("66666666-6666-6666-6666-666666666666") });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_ExternalAuthorId",
                table: "Authors",
                column: "ExternalAuthorId",
                unique: true,
                filter: "[ExternalAuthorId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Authors_UserId",
                table: "Authors",
                column: "UserId",
                unique: true,
                filter: "[UserId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContentAuthors_Authors_AuthorId",
                table: "AuthorContentAuthors",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorSubscriptions_Authors_AuthorId",
                table: "AuthorSubscriptions",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContentAuthors_Authors_AuthorId",
                table: "AuthorContentAuthors");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorSubscriptions_Authors_AuthorId",
                table: "AuthorSubscriptions");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.AddColumn<bool>(
                name: "IsAuthor",
                table: "ApplicationUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ApplicationUsers",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"),
                column: "IsAuthor",
                value: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContentAuthors_ApplicationUsers_AuthorId",
                table: "AuthorContentAuthors",
                column: "AuthorId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorSubscriptions_ApplicationUsers_AuthorId",
                table: "AuthorSubscriptions",
                column: "AuthorId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id");
        }
    }
}

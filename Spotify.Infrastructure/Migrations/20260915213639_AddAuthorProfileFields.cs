using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthorProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Bio",
                table: "Authors",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "BioImageItemId",
                table: "Authors",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MonthList",
                table: "Authors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Authors",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"),
                columns: new[] { "Bio", "BioImageItemId" },
                values: new object[] { null, null });

            migrationBuilder.CreateIndex(
                name: "IX_Authors_BioImageItemId",
                table: "Authors",
                column: "BioImageItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_ImageItems_BioImageItemId",
                table: "Authors",
                column: "BioImageItemId",
                principalTable: "ImageItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_ImageItems_BioImageItemId",
                table: "Authors");

            migrationBuilder.DropIndex(
                name: "IX_Authors_BioImageItemId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "Bio",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "BioImageItemId",
                table: "Authors");

            migrationBuilder.DropColumn(
                name: "MonthList",
                table: "Authors");
        }
    }
}

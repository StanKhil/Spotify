using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Spotify.Infrastructure.Persistance.Context;

#nullable disable

namespace Spotify.Infrastructure.Migrations;

[DbContext(typeof(ApplicationContext))]
[Migration("20260929000000_AddLikeOrder")]
public partial class AddLikeOrder : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Order",
            table: "Likes",
            type: "int",
            nullable: false,
            defaultValue: 0);

        // Existing liked tracks keep their prior display order (oldest like first).
        migrationBuilder.Sql("""
            ;WITH OrderedTrackLikes AS
            (
                SELECT l.Id,
                       ROW_NUMBER() OVER
                       (
                           PARTITION BY l.ApplicationUserId
                           ORDER BY l.LikedAt ASC, l.Id ASC
                       ) AS NewOrder
                FROM Likes AS l
                INNER JOIN AuthorContents AS ac ON ac.Id = l.AuthorContentId
                INNER JOIN AudioContent AS content ON content.Id = ac.ItemId
                WHERE content.Discriminator = 'Track'
            )
            UPDATE l
            SET [Order] = ordered.NewOrder
            FROM Likes AS l
            INNER JOIN OrderedTrackLikes AS ordered ON ordered.Id = l.Id;
            """);

        migrationBuilder.CreateIndex(
            name: "IX_Likes_ApplicationUserId_Order",
            table: "Likes",
            columns: new[] { "ApplicationUserId", "Order" });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Likes_ApplicationUserId_Order",
            table: "Likes");

        migrationBuilder.DropColumn(
            name: "Order",
            table: "Likes");
    }
}

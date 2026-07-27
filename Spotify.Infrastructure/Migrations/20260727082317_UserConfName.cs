using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Spotify.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UserConfName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_UserAccesses_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_UserAccesses_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_UserAccesses_AuthorId",
                table: "AuthorContents");

            migrationBuilder.DropForeignKey(
                name: "FK_LastPlayedEntries_UserAccesses_ApplicationUserId",
                table: "LastPlayedEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_UserAccesses_ApplicationUserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_UserAccesses_ApplicationUserId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Settings_SettingsId",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAccesses_Subscriptions_SubscriptionId",
                table: "UserAccesses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_UserAccesses_UserId",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_UserAccesses_UserId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_UserAccesses_UserId",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAccesses",
                table: "UserAccesses");

            migrationBuilder.RenameTable(
                name: "UserAccesses",
                newName: "ApplicationUsers");

            migrationBuilder.RenameIndex(
                name: "IX_UserAccesses_SubscriptionId",
                table: "ApplicationUsers",
                newName: "IX_ApplicationUsers_SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_UserAccesses_SettingsId",
                table: "ApplicationUsers",
                newName: "IX_ApplicationUsers_SettingsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ApplicationUsers",
                table: "ApplicationUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Settings_SettingsId",
                table: "ApplicationUsers",
                column: "SettingsId",
                principalTable: "Settings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationUsers_Subscriptions_SubscriptionId",
                table: "ApplicationUsers",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_ApplicationUsers_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_ApplicationUsers_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContents_ApplicationUsers_AuthorId",
                table: "AuthorContents",
                column: "AuthorId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LastPlayedEntries_ApplicationUsers_ApplicationUserId",
                table: "LastPlayedEntries",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_ApplicationUsers_ApplicationUserId",
                table: "Likes",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_ApplicationUsers_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_ApplicationUsers_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_ApplicationUsers_UserId",
                table: "UserProfiles",
                column: "UserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_ApplicationUsers_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "ApplicationUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Settings_SettingsId",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationUsers_Subscriptions_SubscriptionId",
                table: "ApplicationUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserLogins_ApplicationUsers_UserId",
                table: "AspNetUserLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUserTokens_ApplicationUsers_UserId",
                table: "AspNetUserTokens");

            migrationBuilder.DropForeignKey(
                name: "FK_AuthorContents_ApplicationUsers_AuthorId",
                table: "AuthorContents");

            migrationBuilder.DropForeignKey(
                name: "FK_LastPlayedEntries_ApplicationUsers_ApplicationUserId",
                table: "LastPlayedEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_Likes_ApplicationUsers_ApplicationUserId",
                table: "Likes");

            migrationBuilder.DropForeignKey(
                name: "FK_Playlists_ApplicationUsers_ApplicationUserId",
                table: "Playlists");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClaims_ApplicationUsers_UserId",
                table: "UserClaims");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_ApplicationUsers_UserId",
                table: "UserProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_ApplicationUsers_UserId",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ApplicationUsers",
                table: "ApplicationUsers");

            migrationBuilder.RenameTable(
                name: "ApplicationUsers",
                newName: "UserAccesses");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUsers_SubscriptionId",
                table: "UserAccesses",
                newName: "IX_UserAccesses_SubscriptionId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicationUsers_SettingsId",
                table: "UserAccesses",
                newName: "IX_UserAccesses_SettingsId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAccesses",
                table: "UserAccesses",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserLogins_UserAccesses_UserId",
                table: "AspNetUserLogins",
                column: "UserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUserTokens_UserAccesses_UserId",
                table: "AspNetUserTokens",
                column: "UserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AuthorContents_UserAccesses_AuthorId",
                table: "AuthorContents",
                column: "AuthorId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LastPlayedEntries_UserAccesses_ApplicationUserId",
                table: "LastPlayedEntries",
                column: "ApplicationUserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Likes_UserAccesses_ApplicationUserId",
                table: "Likes",
                column: "ApplicationUserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Playlists_UserAccesses_ApplicationUserId",
                table: "Playlists",
                column: "ApplicationUserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Settings_SettingsId",
                table: "UserAccesses",
                column: "SettingsId",
                principalTable: "Settings",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAccesses_Subscriptions_SubscriptionId",
                table: "UserAccesses",
                column: "SubscriptionId",
                principalTable: "Subscriptions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClaims_UserAccesses_UserId",
                table: "UserClaims",
                column: "UserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_UserAccesses_UserId",
                table: "UserProfiles",
                column: "UserId",
                principalTable: "UserAccesses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_UserAccesses_UserId",
                table: "UserRoles",
                column: "UserId",
                principalTable: "UserAccesses",
                principalColumn: "Id");
        }
    }
}

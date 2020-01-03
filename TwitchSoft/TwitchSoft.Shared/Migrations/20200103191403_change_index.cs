using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class change_index : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBans_UserId_BannedTime",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_UserId_SubscribedTime",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_CommunitySubscriptions_UserId_Date",
                table: "CommunitySubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_UserId_PostedTime",
                table: "ChatMessages");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_UserId",
                table: "UserBans",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "BannedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "SubscribedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySubscriptions_UserId",
                table: "CommunitySubscriptions",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "PostedTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBans_UserId",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_CommunitySubscriptions_UserId",
                table: "CommunitySubscriptions");

            migrationBuilder.DropIndex(
                name: "IX_ChatMessages_UserId",
                table: "ChatMessages");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_UserId_BannedTime",
                table: "UserBans",
                columns: new[] { "UserId", "BannedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId_SubscribedTime",
                table: "Subscriptions",
                columns: new[] { "UserId", "SubscribedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySubscriptions_UserId_Date",
                table: "CommunitySubscriptions",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId_PostedTime",
                table: "ChatMessages",
                columns: new[] { "UserId", "PostedTime" });
        }
    }
}

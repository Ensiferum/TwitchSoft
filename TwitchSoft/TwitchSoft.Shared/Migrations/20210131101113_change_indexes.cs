using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class change_indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBans_Id",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_ChannelId",
                table: "Subscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_Channel",
                table: "UserBans",
                column: "Channel");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_Id",
                table: "UserBans",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_UserName",
                table: "UserBans",
                column: "UserName");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ChannelId",
                table: "Subscriptions",
                column: "ChannelId")
                .Annotation("SqlServer:Include", new[] { "SubscribedTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserBans_Channel",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_UserBans_Id",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_UserBans_UserName",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_ChannelId",
                table: "Subscriptions");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_Id",
                table: "UserBans",
                column: "Id")
                .Annotation("SqlServer:Include", new[] { "UserName", "Channel", "BannedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ChannelId",
                table: "Subscriptions",
                column: "ChannelId");
        }
    }
}

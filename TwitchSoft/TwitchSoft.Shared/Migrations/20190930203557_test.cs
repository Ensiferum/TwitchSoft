using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class test : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_TrackingChannels_TrackingChannelId",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_TrackingChannelId",
                table: "Subscriptions");

            migrationBuilder.DropColumn(
                name: "TrackingChannelId",
                table: "Subscriptions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TrackingChannelId",
                table: "Subscriptions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_TrackingChannelId",
                table: "Subscriptions",
                column: "TrackingChannelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_TrackingChannels_TrackingChannelId",
                table: "Subscriptions",
                column: "TrackingChannelId",
                principalTable: "TrackingChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

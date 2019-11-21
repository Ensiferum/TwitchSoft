using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class Remove_Track_Channels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_TrackingChannels_ChannelId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunitySubscriptions_TrackingChannels_ChannelId",
                table: "CommunitySubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_TrackingChannels_ChannelId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBans_TrackingChannels_ChannelId",
                table: "UserBans");

            migrationBuilder.AddColumn<bool>(
                name: "JoinChannel",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "TrackMessages",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql(@"
Merge Users as target
using TrackingChannels as source
on
target.Id = source.Id
When matched 
Then
update 
set target.id=source.id,
    target.JoinChannel = 1,
	target.TrackMessages = 1
When not matched by Target Then
INSERT (id, Username, JoinChannel, TrackMessages) VALUES (id, ChannelName, 1, 1);
");

            migrationBuilder.DropTable(
                name: "TrackingChannels");

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_Users_ChannelId",
                table: "ChatMessages",
                column: "ChannelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunitySubscriptions_Users_ChannelId",
                table: "CommunitySubscriptions",
                column: "ChannelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_Users_ChannelId",
                table: "Subscriptions",
                column: "ChannelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBans_Users_ChannelId",
                table: "UserBans",
                column: "ChannelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChatMessages_Users_ChannelId",
                table: "ChatMessages");

            migrationBuilder.DropForeignKey(
                name: "FK_CommunitySubscriptions_Users_ChannelId",
                table: "CommunitySubscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_Subscriptions_Users_ChannelId",
                table: "Subscriptions");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBans_Users_ChannelId",
                table: "UserBans");

            migrationBuilder.DropColumn(
                name: "JoinChannel",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "TrackMessages",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "TrackingChannels",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false),
                    ChannelName = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingChannels", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrackingChannels_Id",
                table: "TrackingChannels",
                column: "Id",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChatMessages_TrackingChannels_ChannelId",
                table: "ChatMessages",
                column: "ChannelId",
                principalTable: "TrackingChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CommunitySubscriptions_TrackingChannels_ChannelId",
                table: "CommunitySubscriptions",
                column: "ChannelId",
                principalTable: "TrackingChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Subscriptions_TrackingChannels_ChannelId",
                table: "Subscriptions",
                column: "ChannelId",
                principalTable: "TrackingChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBans_TrackingChannels_ChannelId",
                table: "UserBans",
                column: "ChannelId",
                principalTable: "TrackingChannels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

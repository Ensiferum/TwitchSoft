using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class initial_v2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrackingChannels",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    ChannelName = table.Column<string>(maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrackingChannels", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false),
                    Username = table.Column<string>(maxLength: 60, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false),
                    Message = table.Column<string>(maxLength: 1024, nullable: true),
                    PostedTime = table.Column<DateTime>(nullable: false),
                    IsBroadcaster = table.Column<bool>(nullable: false),
                    IsSubscriber = table.Column<bool>(nullable: false),
                    IsModerator = table.Column<bool>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    UserType = table.Column<byte>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_TrackingChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "TrackingChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChatMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommunitySubscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    GiftCount = table.Column<int>(nullable: false),
                    SubscriptionPlan = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommunitySubscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommunitySubscriptions_TrackingChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "TrackingChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommunitySubscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<long>(nullable: false),
                    UserType = table.Column<byte>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false),
                    SubscriptionPlan = table.Column<int>(nullable: false),
                    SubscribedTime = table.Column<DateTime>(nullable: false),
                    Months = table.Column<int>(nullable: false),
                    GiftedBy = table.Column<long>(nullable: true),
                    TrackingChannelId = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_TrackingChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "TrackingChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_GiftedBy",
                        column: x => x.GiftedBy,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_TrackingChannels_TrackingChannelId",
                        column: x => x.TrackingChannelId,
                        principalTable: "TrackingChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserBans",
                columns: table => new
                {
                    UserId = table.Column<long>(nullable: false),
                    BannedTime = table.Column<DateTime>(nullable: false),
                    ChannelId = table.Column<long>(nullable: false),
                    Reason = table.Column<string>(maxLength: 140, nullable: true),
                    Duration = table.Column<int>(nullable: true),
                    BanType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserBans", x => new { x.UserId, x.BannedTime });
                    table.ForeignKey(
                        name: "FK_UserBans_TrackingChannels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "TrackingChannels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserBans_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ChannelId",
                table: "ChatMessages",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_Id",
                table: "ChatMessages",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_UserId_PostedTime",
                table: "ChatMessages",
                columns: new[] { "UserId", "PostedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySubscriptions_ChannelId",
                table: "CommunitySubscriptions",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySubscriptions_Id",
                table: "CommunitySubscriptions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySubscriptions_UserId_Date",
                table: "CommunitySubscriptions",
                columns: new[] { "UserId", "Date" });

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_ChannelId",
                table: "Subscriptions",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_GiftedBy",
                table: "Subscriptions",
                column: "GiftedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Id",
                table: "Subscriptions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_TrackingChannelId",
                table: "Subscriptions",
                column: "TrackingChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId_SubscribedTime",
                table: "Subscriptions",
                columns: new[] { "UserId", "SubscribedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_TrackingChannels_Id",
                table: "TrackingChannels",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_ChannelId",
                table: "UserBans",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_UserId_BannedTime",
                table: "UserBans",
                columns: new[] { "UserId", "BannedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "CommunitySubscriptions");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "UserBans");

            migrationBuilder.DropTable(
                name: "TrackingChannels");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

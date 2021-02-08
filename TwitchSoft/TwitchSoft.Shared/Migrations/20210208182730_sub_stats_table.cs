using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class sub_stats_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Id",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_UserBans_Id",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_Subscriptions_Id",
                table: "Subscriptions");

            migrationBuilder.DropIndex(
                name: "IX_CommunitySubscriptions_Id",
                table: "CommunitySubscriptions");

            migrationBuilder.CreateTable(
                name: "SubscriptionStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<long>(type: "bigint", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionStatistics", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionStatistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionStatistics_UserId",
                table: "SubscriptionStatistics",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubscriptionStatistics");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Id",
                table: "Users",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_Id",
                table: "UserBans",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_Id",
                table: "Subscriptions",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommunitySubscriptions_Id",
                table: "CommunitySubscriptions",
                column: "Id",
                unique: true);
        }
    }
}

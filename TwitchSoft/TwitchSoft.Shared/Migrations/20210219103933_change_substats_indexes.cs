using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class change_substats_indexes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionStatistics",
                table: "SubscriptionStatistics");

            migrationBuilder.DropIndex(
                name: "IX_SubscriptionStatistics_UserId",
                table: "SubscriptionStatistics");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "SubscriptionStatistics");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionStatistics",
                table: "SubscriptionStatistics",
                columns: new[] { "UserId", "Date" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_SubscriptionStatistics",
                table: "SubscriptionStatistics");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "SubscriptionStatistics",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubscriptionStatistics",
                table: "SubscriptionStatistics",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionStatistics_UserId",
                table: "SubscriptionStatistics",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "Date" });
        }
    }
}

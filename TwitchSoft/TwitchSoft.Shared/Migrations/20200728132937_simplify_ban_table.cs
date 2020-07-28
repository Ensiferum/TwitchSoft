using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class simplify_ban_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserBans_Users_ChannelId",
                table: "UserBans");

            migrationBuilder.DropForeignKey(
                name: "FK_UserBans_Users_UserId",
                table: "UserBans");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBans",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_UserBans_ChannelId",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_UserBans_UserId",
                table: "UserBans");

            migrationBuilder.DropColumn(
                name: "ChannelId",
                table: "UserBans");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserBans");

            migrationBuilder.AddColumn<long>(
                name: "Id",
                table: "UserBans",
                type: "bigint",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Channel",
                table: "UserBans",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "UserBans",
                type: "nvarchar(60)",
                maxLength: 60,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBans",
                table: "UserBans",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_Id",
                table: "UserBans",
                column: "Id")
                .Annotation("SqlServer:Include", new[] { "UserName", "Channel", "BannedTime" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserBans",
                table: "UserBans");

            migrationBuilder.DropIndex(
                name: "IX_UserBans_Id",
                table: "UserBans");

            migrationBuilder.DropColumn(
                name: "Channel",
                table: "UserBans");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "UserBans");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserBans",
                newName: "UserId");

            migrationBuilder.AlterColumn<long>(
                name: "UserId",
                table: "UserBans",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<long>(
                name: "ChannelId",
                table: "UserBans",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserBans",
                table: "UserBans",
                columns: new[] { "UserId", "BannedTime" });

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_ChannelId",
                table: "UserBans",
                column: "ChannelId");

            migrationBuilder.CreateIndex(
                name: "IX_UserBans_UserId",
                table: "UserBans",
                column: "UserId")
                .Annotation("SqlServer:Include", new[] { "BannedTime" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserBans_Users_ChannelId",
                table: "UserBans",
                column: "ChannelId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserBans_Users_UserId",
                table: "UserBans",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

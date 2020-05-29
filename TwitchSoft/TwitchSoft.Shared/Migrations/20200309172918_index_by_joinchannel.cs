using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class index_by_joinchannel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Users_JoinChannel",
                table: "Users",
                column: "JoinChannel");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_JoinChannel",
                table: "Users");
        }
    }
}

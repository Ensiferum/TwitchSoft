using Microsoft.EntityFrameworkCore.Migrations;

namespace TwitchSoft.Shared.Migrations
{
    public partial class remove_track_messages_prop : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrackMessages",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "TrackMessages",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

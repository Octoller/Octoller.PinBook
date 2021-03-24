using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Octoller.PinBook.Web.Data.Migrations
{
    public partial class removeAvatarForProfileModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "Profiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Avatar",
                table: "Profiles",
                type: "varbinary(max)",
                nullable: true);
        }
    }
}

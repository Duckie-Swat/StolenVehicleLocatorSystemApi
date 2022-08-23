using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StolenVehicleLocatorSystem.DataAccessor.Migrations
{
    public partial class updateuserremoveaddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AspNetUsers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AspNetUsers",
                type: "text",
                nullable: true);
        }
    }
}

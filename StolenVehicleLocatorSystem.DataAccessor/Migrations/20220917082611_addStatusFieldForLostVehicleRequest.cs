using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StolenVehicleLocatorSystem.DataAccessor.Migrations
{
    public partial class addStatusFieldForLostVehicleRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "LostVehicleRequests",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "LostVehicleRequests");
        }
    }
}

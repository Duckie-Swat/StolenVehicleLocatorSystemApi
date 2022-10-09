using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StolenVehicleLocatorSystem.DataAccessor.Migrations
{
    public partial class addReferenceFromCameraDetectedResultToLostVehicleRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "LostVehicleRequestId",
                table: "CameraDetectedResult",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "PlateNumber",
                table: "CameraDetectedResult",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CameraDetectedResult_LostVehicleRequestId",
                table: "CameraDetectedResult",
                column: "LostVehicleRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_CameraDetectedResult_LostVehicleRequests_LostVehicleRequest~",
                table: "CameraDetectedResult",
                column: "LostVehicleRequestId",
                principalTable: "LostVehicleRequests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CameraDetectedResult_LostVehicleRequests_LostVehicleRequest~",
                table: "CameraDetectedResult");

            migrationBuilder.DropIndex(
                name: "IX_CameraDetectedResult_LostVehicleRequestId",
                table: "CameraDetectedResult");

            migrationBuilder.DropColumn(
                name: "LostVehicleRequestId",
                table: "CameraDetectedResult");

            migrationBuilder.DropColumn(
                name: "PlateNumber",
                table: "CameraDetectedResult");
        }
    }
}

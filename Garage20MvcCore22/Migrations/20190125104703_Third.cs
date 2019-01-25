using Microsoft.EntityFrameworkCore.Migrations;

namespace Garage20MvcCore22.Migrations
{
    public partial class Third : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Parked",
                table: "ParkedVehicle",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Parked",
                table: "ParkedVehicle");
        }
    }
}

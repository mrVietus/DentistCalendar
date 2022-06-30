using Microsoft.EntityFrameworkCore.Migrations;

namespace DentistCalendar.Persistence.Migrations
{
    public partial class UpdateMenuElementEntitySecond : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ParentMenuId",
                table: "MenuElements",
                newName: "ParentMenuElementId");

            migrationBuilder.AddColumn<int>(
                name: "MenuElementId",
                table: "MenuElements",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MenuElementId",
                table: "MenuElements");

            migrationBuilder.RenameColumn(
                name: "ParentMenuElementId",
                table: "MenuElements",
                newName: "ParentMenuId");
        }
    }
}

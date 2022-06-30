using Microsoft.EntityFrameworkCore.Migrations;

namespace DentistCalendar.Persistence.Migrations
{
    public partial class UpdateAdminWithNIP : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NIP",
                table: "Admins",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NIP",
                table: "Admins");
        }
    }
}

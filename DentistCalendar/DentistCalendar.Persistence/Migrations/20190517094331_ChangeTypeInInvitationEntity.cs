using Microsoft.EntityFrameworkCore.Migrations;

namespace DentistCalendar.Persistence.Migrations
{
    public partial class ChangeTypeInInvitationEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "InvitingDentistOfficeId",
                table: "Invitations",
                nullable: true,
                oldClrType: typeof(int));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "InvitingDentistOfficeId",
                table: "Invitations",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}

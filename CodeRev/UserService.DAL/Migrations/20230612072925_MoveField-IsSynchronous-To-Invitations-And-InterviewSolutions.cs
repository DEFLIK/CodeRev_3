using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.DAL.Migrations
{
    public partial class MoveFieldIsSynchronousToInvitationsAndInterviewSolutions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynchronous",
                table: "Interviews");

            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronous",
                table: "Invitations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronous",
                table: "InterviewSolutions",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynchronous",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "IsSynchronous",
                table: "InterviewSolutions");

            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronous",
                table: "Interviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}

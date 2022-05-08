using Microsoft.EntityFrameworkCore.Migrations;

namespace Bua.CodeRev.UserService.DAL.Migrations
{
    public partial class Renaming : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTimeMillis",
                table: "InterviewSolutions",
                newName: "TimeToCheckMs");

            migrationBuilder.RenameColumn(
                name: "EndTimeMillis",
                table: "InterviewSolutions",
                newName: "StartTimeMs");

            migrationBuilder.AddColumn<long>(
                name: "EndTimeMs",
                table: "InterviewSolutions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Vacancy",
                table: "Interviews",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTimeMs",
                table: "InterviewSolutions");

            migrationBuilder.DropColumn(
                name: "Vacancy",
                table: "Interviews");

            migrationBuilder.RenameColumn(
                name: "TimeToCheckMs",
                table: "InterviewSolutions",
                newName: "StartTimeMillis");

            migrationBuilder.RenameColumn(
                name: "StartTimeMs",
                table: "InterviewSolutions",
                newName: "EndTimeMillis");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace Bua.CodeRev.UserService.DAL.Migrations
{
    public partial class AverageGradeAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TaskGrade",
                table: "TaskSolutions",
                newName: "Grade");

            migrationBuilder.AddColumn<int>(
                name: "AverageGrade",
                table: "InterviewSolutions",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageGrade",
                table: "InterviewSolutions");

            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "TaskSolutions",
                newName: "TaskGrade");
        }
    }
}

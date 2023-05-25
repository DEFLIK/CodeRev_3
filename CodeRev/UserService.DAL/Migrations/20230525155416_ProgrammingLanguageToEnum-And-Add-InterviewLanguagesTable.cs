using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.DAL.Migrations
{
    public partial class ProgrammingLanguageToEnumAndAddInterviewLanguagesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProgrammingLanguage",
                table: "Interviews");

            migrationBuilder.AddColumn<int>(
                name: "ProgrammingLanguage",
                table: "Tasks",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "InterviewLanguages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    ProgrammingLanguage = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewLanguages", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewLanguages");

            migrationBuilder.DropColumn(
                name: "ProgrammingLanguage",
                table: "Tasks");

            migrationBuilder.AddColumn<string>(
                name: "ProgrammingLanguage",
                table: "Interviews",
                type: "text",
                nullable: true);
        }
    }
}

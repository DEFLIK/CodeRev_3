using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.DAL.Migrations
{
    public partial class AddFieldTestsCodeToTasksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TestsCode",
                table: "Tasks",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestsCode",
                table: "Tasks");
        }
    }
}

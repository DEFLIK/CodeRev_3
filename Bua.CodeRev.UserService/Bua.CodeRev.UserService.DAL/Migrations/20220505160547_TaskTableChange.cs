using Microsoft.EntityFrameworkCore.Migrations;

namespace Bua.CodeRev.UserService.DAL.Migrations
{
    public partial class TaskTableChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StartCode",
                table: "Tasks",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StartCode",
                table: "Tasks");
        }
    }
}

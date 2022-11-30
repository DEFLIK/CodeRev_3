using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.DAL.Migrations
{
    public partial class AddFieldIsSynchronousToInterviewsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSynchronous",
                table: "Interviews",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSynchronous",
                table: "Interviews");
        }
    }
}

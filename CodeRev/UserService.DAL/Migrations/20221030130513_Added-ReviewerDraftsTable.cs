using System;
using Microsoft.EntityFrameworkCore.Migrations;
using UserService.DAL.Models.Draft;

namespace UserService.DAL.Migrations
{
    public partial class AddedReviewerDraftsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ReviewerDraftId",
                table: "InterviewSolutions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ReviewerDrafts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewSolutionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Draft = table.Column<Draft>(type: "jsonb", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewerDrafts", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewerDrafts");

            migrationBuilder.DropColumn(
                name: "ReviewerDraftId",
                table: "InterviewSolutions");
        }
    }
}

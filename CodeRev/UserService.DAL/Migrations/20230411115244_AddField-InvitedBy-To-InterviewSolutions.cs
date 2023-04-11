using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UserService.DAL.Migrations
{
    public partial class AddFieldInvitedByToInterviewSolutions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "InvitedBy",
                table: "InterviewSolutions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvitedBy",
                table: "InterviewSolutions");
        }
    }
}

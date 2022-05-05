using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Bua.CodeRev.UserService.DAL.Migrations
{
    public partial class RemoveTimelinesIds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RunTimelineId",
                table: "TaskSolutions");

            migrationBuilder.DropColumn(
                name: "TimelineId",
                table: "TaskSolutions");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RunTimelineId",
                table: "TaskSolutions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "TimelineId",
                table: "TaskSolutions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}

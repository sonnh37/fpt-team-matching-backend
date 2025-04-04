using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateTeamMemberAndNoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Attitude",
                table: "TeamMember",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Notification",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ProjectId",
                table: "Notification",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_Project_ProjectId",
                table: "Notification",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_Project_ProjectId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ProjectId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "Attitude",
                table: "TeamMember");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Notification");
        }
    }
}

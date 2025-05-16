using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class addSemesterIdForTopic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SemesterId",
                table: "Topic",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Topic_SemesterId",
                table: "Topic",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Semester_SemesterId",
                table: "Topic",
                column: "SemesterId",
                principalTable: "Semester",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Semester_SemesterId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Topic_SemesterId",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "Topic");
        }
    }
}

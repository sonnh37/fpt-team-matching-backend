using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserXRoleSemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsPrimary",
                table: "UserXRole",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "SemesterId",
                table: "UserXRole",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserXRole_SemesterId",
                table: "UserXRole",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserXRole_Semester_SemesterId",
                table: "UserXRole",
                column: "SemesterId",
                principalTable: "Semester",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserXRole_Semester_SemesterId",
                table: "UserXRole");

            migrationBuilder.DropIndex(
                name: "IX_UserXRole_SemesterId",
                table: "UserXRole");

            migrationBuilder.DropColumn(
                name: "IsPrimary",
                table: "UserXRole");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "UserXRole");
        }
    }
}

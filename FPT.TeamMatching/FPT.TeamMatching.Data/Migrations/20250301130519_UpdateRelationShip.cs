using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Major",
                table: "ProfileStudent");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "ProfileStudent");

            migrationBuilder.AddColumn<Guid>(
                name: "SemesterId",
                table: "ProfileStudent",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialtyId",
                table: "ProfileStudent",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectId",
                table: "Blog",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProfileStudent_SemesterId",
                table: "ProfileStudent",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileStudent_SpecialtyId",
                table: "ProfileStudent",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Blog_ProjectId",
                table: "Blog",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_Project_ProjectId",
                table: "Blog",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileStudent_Semester_SemesterId",
                table: "ProfileStudent",
                column: "SemesterId",
                principalTable: "Semester",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileStudent_Specialty_SpecialtyId",
                table: "ProfileStudent",
                column: "SpecialtyId",
                principalTable: "Specialty",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_Project_ProjectId",
                table: "Blog");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileStudent_Semester_SemesterId",
                table: "ProfileStudent");

            migrationBuilder.DropForeignKey(
                name: "FK_ProfileStudent_Specialty_SpecialtyId",
                table: "ProfileStudent");

            migrationBuilder.DropIndex(
                name: "IX_ProfileStudent_SemesterId",
                table: "ProfileStudent");

            migrationBuilder.DropIndex(
                name: "IX_ProfileStudent_SpecialtyId",
                table: "ProfileStudent");

            migrationBuilder.DropIndex(
                name: "IX_Blog_ProjectId",
                table: "Blog");

            migrationBuilder.DropColumn(
                name: "SemesterId",
                table: "ProfileStudent");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "ProfileStudent");

            migrationBuilder.DropColumn(
                name: "ProjectId",
                table: "Blog");

            migrationBuilder.AddColumn<string>(
                name: "Major",
                table: "ProfileStudent",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Semester",
                table: "ProfileStudent",
                type: "text",
                nullable: true);
        }
    }
}

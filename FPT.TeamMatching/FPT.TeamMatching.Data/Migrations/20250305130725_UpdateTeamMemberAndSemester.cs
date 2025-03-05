using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTeamMemberAndSemester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "TeamMember",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SemesterPrefixName",
                table: "Semester",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "TeamMember");

            migrationBuilder.DropColumn(
                name: "SemesterPrefixName",
                table: "Semester");
        }
    }
}

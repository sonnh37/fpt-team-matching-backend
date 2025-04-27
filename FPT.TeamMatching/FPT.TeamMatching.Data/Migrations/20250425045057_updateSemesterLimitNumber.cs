using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateSemesterLimitNumber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LimitTopicMentorOnly",
                table: "Semester",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LimitTopicSubMentor",
                table: "Semester",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LimitTopicMentorOnly",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "LimitTopicSubMentor",
                table: "Semester");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateSemesterfield : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublicIdeaDate",
                table: "Semester",
                newName: "PublicTopicDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PublicTopicDate",
                table: "Semester",
                newName: "PublicIdeaDate");
        }
    }
}

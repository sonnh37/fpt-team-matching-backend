using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Criteria");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Criteria",
                newName: "Question");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Question",
                table: "Criteria",
                newName: "Name");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Criteria",
                type: "text",
                nullable: true);
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateTeamMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommentDefense1",
                table: "TeamMember",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CommentDefense2",
                table: "TeamMember",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommentDefense1",
                table: "TeamMember");

            migrationBuilder.DropColumn(
                name: "CommentDefense2",
                table: "TeamMember");
        }
    }
}

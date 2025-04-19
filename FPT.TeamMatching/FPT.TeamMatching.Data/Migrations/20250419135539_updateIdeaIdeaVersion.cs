using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateIdeaIdeaVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abbreviations",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "EnglishName",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "EnterpriseName",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "File",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "MaxTeamSize",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "VietNamName",
                table: "Idea");

            migrationBuilder.AddColumn<string>(
                name: "Abbreviations",
                table: "IdeaVersion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "IdeaVersion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnglishName",
                table: "IdeaVersion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnterpriseName",
                table: "IdeaVersion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File",
                table: "IdeaVersion",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TeamSize",
                table: "IdeaVersion",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VietNamName",
                table: "IdeaVersion",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abbreviations",
                table: "IdeaVersion");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "IdeaVersion");

            migrationBuilder.DropColumn(
                name: "EnglishName",
                table: "IdeaVersion");

            migrationBuilder.DropColumn(
                name: "EnterpriseName",
                table: "IdeaVersion");

            migrationBuilder.DropColumn(
                name: "File",
                table: "IdeaVersion");

            migrationBuilder.DropColumn(
                name: "TeamSize",
                table: "IdeaVersion");

            migrationBuilder.DropColumn(
                name: "VietNamName",
                table: "IdeaVersion");

            migrationBuilder.AddColumn<string>(
                name: "Abbreviations",
                table: "Idea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Idea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnglishName",
                table: "Idea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnterpriseName",
                table: "Idea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "File",
                table: "Idea",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxTeamSize",
                table: "Idea",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VietNamName",
                table: "Idea",
                type: "text",
                nullable: true);
        }
    }
}

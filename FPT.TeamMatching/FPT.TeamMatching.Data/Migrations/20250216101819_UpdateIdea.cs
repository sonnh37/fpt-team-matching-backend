using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Idea",
                newName: "EnterpriseName");

            migrationBuilder.AlterColumn<bool>(
                name: "IsExistedTeam",
                table: "Idea",
                type: "boolean",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnterpriseTopic",
                table: "Idea",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEnterpriseTopic",
                table: "Idea");

            migrationBuilder.RenameColumn(
                name: "EnterpriseName",
                table: "Idea",
                newName: "Title");

            migrationBuilder.AlterColumn<bool>(
                name: "IsExistedTeam",
                table: "Idea",
                type: "boolean",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "boolean");
        }
    }
}

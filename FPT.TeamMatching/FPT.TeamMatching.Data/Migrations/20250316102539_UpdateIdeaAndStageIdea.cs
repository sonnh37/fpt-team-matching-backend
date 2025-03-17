using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdeaAndStageIdea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Idea_Semester_SemesterId",
                table: "Idea");

            migrationBuilder.RenameColumn(
                name: "SemesterId",
                table: "Idea",
                newName: "StageIdeaId");

            migrationBuilder.RenameIndex(
                name: "IX_Idea_SemesterId",
                table: "Idea",
                newName: "IX_Idea_StageIdeaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Idea_StageIdea_StageIdeaId",
                table: "Idea",
                column: "StageIdeaId",
                principalTable: "StageIdea",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Idea_StageIdea_StageIdeaId",
                table: "Idea");

            migrationBuilder.RenameColumn(
                name: "StageIdeaId",
                table: "Idea",
                newName: "SemesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Idea_StageIdeaId",
                table: "Idea",
                newName: "IX_Idea_SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Idea_Semester_SemesterId",
                table: "Idea",
                column: "SemesterId",
                principalTable: "Semester",
                principalColumn: "Id");
        }
    }
}

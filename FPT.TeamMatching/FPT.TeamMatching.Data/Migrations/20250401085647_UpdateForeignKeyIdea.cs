using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKeyIdea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaHistory_Idea_IdeaId",
                table: "IdeaHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaRequest_Idea_IdeaId",
                table: "IdeaRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorIdeaRequest_Idea_IdeaId",
                table: "MentorIdeaRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Idea_IdeaId",
                table: "Project");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaHistory_Idea_IdeaId",
                table: "IdeaHistory",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaRequest_Idea_IdeaId",
                table: "IdeaRequest",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorIdeaRequest_Idea_IdeaId",
                table: "MentorIdeaRequest",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Idea_IdeaId",
                table: "Project",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaHistory_Idea_IdeaId",
                table: "IdeaHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaRequest_Idea_IdeaId",
                table: "IdeaRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorIdeaRequest_Idea_IdeaId",
                table: "MentorIdeaRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Idea_IdeaId",
                table: "Project");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaHistory_Idea_IdeaId",
                table: "IdeaHistory",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaRequest_Idea_IdeaId",
                table: "IdeaRequest",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MentorIdeaRequest_Idea_IdeaId",
                table: "MentorIdeaRequest",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Idea_IdeaId",
                table: "Project",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id");
        }
    }
}

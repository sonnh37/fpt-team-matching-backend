using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateAnswerCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaRequestId",
                table: "AnswerCriteria");

            migrationBuilder.RenameColumn(
                name: "IdeaRequestId",
                table: "AnswerCriteria",
                newName: "IdeaVersionRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerCriteria_IdeaRequestId",
                table: "AnswerCriteria",
                newName: "IX_AnswerCriteria_IdeaVersionRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaVersionRequestId",
                table: "AnswerCriteria",
                column: "IdeaVersionRequestId",
                principalTable: "IdeaVersionRequest",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaVersionRequestId",
                table: "AnswerCriteria");

            migrationBuilder.RenameColumn(
                name: "IdeaVersionRequestId",
                table: "AnswerCriteria",
                newName: "IdeaRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerCriteria_IdeaVersionRequestId",
                table: "AnswerCriteria",
                newName: "IX_AnswerCriteria_IdeaRequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaRequestId",
                table: "AnswerCriteria",
                column: "IdeaRequestId",
                principalTable: "IdeaVersionRequest",
                principalColumn: "Id");
        }
    }
}

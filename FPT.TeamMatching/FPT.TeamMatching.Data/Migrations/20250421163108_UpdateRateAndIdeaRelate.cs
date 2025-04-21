using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRateAndIdeaRelate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaVersionRequestId",
                table: "AnswerCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaVersion_Idea_IdeaId",
                table: "IdeaVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaVersionRequest_IdeaVersion_IdeaVersionId",
                table: "IdeaVersionRequest");

            migrationBuilder.RenameColumn(
                name: "NumbOfStar",
                table: "Rate",
                newName: "PercentContribution");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaVersionRequestId",
                table: "AnswerCriteria",
                column: "IdeaVersionRequestId",
                principalTable: "IdeaVersionRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaVersion_Idea_IdeaId",
                table: "IdeaVersion",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaVersionRequest_IdeaVersion_IdeaVersionId",
                table: "IdeaVersionRequest",
                column: "IdeaVersionId",
                principalTable: "IdeaVersion",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaVersionRequestId",
                table: "AnswerCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaVersion_Idea_IdeaId",
                table: "IdeaVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaVersionRequest_IdeaVersion_IdeaVersionId",
                table: "IdeaVersionRequest");

            migrationBuilder.RenameColumn(
                name: "PercentContribution",
                table: "Rate",
                newName: "NumbOfStar");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaVersionRequestId",
                table: "AnswerCriteria",
                column: "IdeaVersionRequestId",
                principalTable: "IdeaVersionRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaVersion_Idea_IdeaId",
                table: "IdeaVersion",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaVersionRequest_IdeaVersion_IdeaVersionId",
                table: "IdeaVersionRequest",
                column: "IdeaVersionId",
                principalTable: "IdeaVersion",
                principalColumn: "Id");
        }
    }
}

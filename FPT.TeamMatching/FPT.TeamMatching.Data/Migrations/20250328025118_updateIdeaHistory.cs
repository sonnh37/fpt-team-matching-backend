using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateIdeaHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaHistory_User_CouncilId",
                table: "IdeaHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaHistoryRequest_IdeaHistory_IdeaHistoryId",
                table: "IdeaHistoryRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaHistoryRequest_User_ReviewerId",
                table: "IdeaHistoryRequest");

            migrationBuilder.DropIndex(
                name: "IX_IdeaHistoryRequest_IdeaHistoryId",
                table: "IdeaHistoryRequest");

            migrationBuilder.DropIndex(
                name: "IX_IdeaHistoryRequest_ReviewerId",
                table: "IdeaHistoryRequest");

            migrationBuilder.RenameColumn(
                name: "CouncilId",
                table: "IdeaHistory",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_IdeaHistory_CouncilId",
                table: "IdeaHistory",
                newName: "IX_IdeaHistory_UserId");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "IdeaHistoryRequest",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistoryRequest_UserId",
                table: "IdeaHistoryRequest",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaHistory_User_UserId",
                table: "IdeaHistory",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaHistoryRequest_User_UserId",
                table: "IdeaHistoryRequest",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaHistory_User_UserId",
                table: "IdeaHistory");

            migrationBuilder.DropForeignKey(
                name: "FK_IdeaHistoryRequest_User_UserId",
                table: "IdeaHistoryRequest");

            migrationBuilder.DropIndex(
                name: "IX_IdeaHistoryRequest_UserId",
                table: "IdeaHistoryRequest");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "IdeaHistoryRequest");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "IdeaHistory",
                newName: "CouncilId");

            migrationBuilder.RenameIndex(
                name: "IX_IdeaHistory_UserId",
                table: "IdeaHistory",
                newName: "IX_IdeaHistory_CouncilId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistoryRequest_IdeaHistoryId",
                table: "IdeaHistoryRequest",
                column: "IdeaHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistoryRequest_ReviewerId",
                table: "IdeaHistoryRequest",
                column: "ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaHistory_User_CouncilId",
                table: "IdeaHistory",
                column: "CouncilId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaHistoryRequest_IdeaHistory_IdeaHistoryId",
                table: "IdeaHistoryRequest",
                column: "IdeaHistoryId",
                principalTable: "IdeaHistory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaHistoryRequest_User_ReviewerId",
                table: "IdeaHistoryRequest",
                column: "ReviewerId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}

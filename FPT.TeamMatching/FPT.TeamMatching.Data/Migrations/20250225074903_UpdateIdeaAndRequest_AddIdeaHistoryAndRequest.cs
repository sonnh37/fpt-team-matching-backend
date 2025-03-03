using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIdeaAndRequest_AddIdeaHistoryAndRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Idea_User_UserId",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Project");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Idea",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_Idea_UserId",
                table: "Idea",
                newName: "IX_Idea_OwnerId");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "IdeaRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MentorId",
                table: "Idea",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdeaHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    CouncilId = table.Column<Guid>(type: "uuid", nullable: true),
                    FileUpdate = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    ReviewStage = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaHistory_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdeaHistory_User_CouncilId",
                        column: x => x.CouncilId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdeaHistoryRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaHistoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Content = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    ProcessDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaHistoryRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaHistoryRequest_IdeaHistory_IdeaHistoryId",
                        column: x => x.IdeaHistoryId,
                        principalTable: "IdeaHistory",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdeaHistoryRequest_User_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Idea_MentorId",
                table: "Idea",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistory_CouncilId",
                table: "IdeaHistory",
                column: "CouncilId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistory_IdeaId",
                table: "IdeaHistory",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistoryRequest_IdeaHistoryId",
                table: "IdeaHistoryRequest",
                column: "IdeaHistoryId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistoryRequest_ReviewerId",
                table: "IdeaHistoryRequest",
                column: "ReviewerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Idea_User_MentorId",
                table: "Idea",
                column: "MentorId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Idea_User_OwnerId",
                table: "Idea",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Idea_User_MentorId",
                table: "Idea");

            migrationBuilder.DropForeignKey(
                name: "FK_Idea_User_OwnerId",
                table: "Idea");

            migrationBuilder.DropTable(
                name: "IdeaHistoryRequest");

            migrationBuilder.DropTable(
                name: "IdeaHistory");

            migrationBuilder.DropIndex(
                name: "IX_Idea_MentorId",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "IdeaRequest");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "Idea");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "Idea",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Idea_OwnerId",
                table: "Idea",
                newName: "IX_Idea_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Project",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Idea_User_UserId",
                table: "Idea",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class addCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Idea_IdeaId",
                table: "Project");

            migrationBuilder.DropTable(
                name: "IdeaHistory");

            migrationBuilder.DropTable(
                name: "IdeaHistoryRequest");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "IdeaRequest");

            migrationBuilder.DropColumn(
                name: "IdeaCode",
                table: "Idea");

            migrationBuilder.RenameColumn(
                name: "IdeaId",
                table: "Project",
                newName: "TopicId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_IdeaId",
                table: "Project",
                newName: "IX_Project_TopicId");

            migrationBuilder.AddColumn<Guid>(
                name: "CriteriaFormId",
                table: "Semester",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberReviewer",
                table: "Semester",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CriteriaFormId",
                table: "IdeaRequest",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Criteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    ValueType = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Criteria", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CriteriaForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Title = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaForm", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Topic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    TopicCode = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Topic_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AnswerCriteria",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaRequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    CriteriaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnswerCriteria", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnswerCriteria_Criteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "Criteria",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AnswerCriteria_IdeaRequest_IdeaRequestId",
                        column: x => x.IdeaRequestId,
                        principalTable: "IdeaRequest",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CriteriaXCriteriaForm",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CriteriaFormId = table.Column<Guid>(type: "uuid", nullable: true),
                    CriteriaId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CriteriaXCriteriaForm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CriteriaXCriteriaForm_CriteriaForm_CriteriaFormId",
                        column: x => x.CriteriaFormId,
                        principalTable: "CriteriaForm",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CriteriaXCriteriaForm_Criteria_CriteriaId",
                        column: x => x.CriteriaId,
                        principalTable: "Criteria",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TopicVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: true),
                    FileUpdate = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    ReviewStage = table.Column<int>(type: "integer", nullable: false),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicVersion_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TopicVersion_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TopicVersion_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Semester_CriteriaFormId",
                table: "Semester",
                column: "CriteriaFormId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaRequest_CriteriaFormId",
                table: "IdeaRequest",
                column: "CriteriaFormId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerCriteria_CriteriaId",
                table: "AnswerCriteria",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerCriteria_IdeaRequestId",
                table: "AnswerCriteria",
                column: "IdeaRequestId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaXCriteriaForm_CriteriaFormId",
                table: "CriteriaXCriteriaForm",
                column: "CriteriaFormId");

            migrationBuilder.CreateIndex(
                name: "IX_CriteriaXCriteriaForm_CriteriaId",
                table: "CriteriaXCriteriaForm",
                column: "CriteriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_IdeaId",
                table: "Topic",
                column: "IdeaId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TopicVersion_IdeaId",
                table: "TopicVersion",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicVersion_TopicId",
                table: "TopicVersion",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicVersion_UserId",
                table: "TopicVersion",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_IdeaRequest_CriteriaForm_CriteriaFormId",
                table: "IdeaRequest",
                column: "CriteriaFormId",
                principalTable: "CriteriaForm",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Topic_TopicId",
                table: "Project",
                column: "TopicId",
                principalTable: "Topic",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Semester_CriteriaForm_CriteriaFormId",
                table: "Semester",
                column: "CriteriaFormId",
                principalTable: "CriteriaForm",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_IdeaRequest_CriteriaForm_CriteriaFormId",
                table: "IdeaRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Topic_TopicId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Semester_CriteriaForm_CriteriaFormId",
                table: "Semester");

            migrationBuilder.DropTable(
                name: "AnswerCriteria");

            migrationBuilder.DropTable(
                name: "CriteriaXCriteriaForm");

            migrationBuilder.DropTable(
                name: "TopicVersion");

            migrationBuilder.DropTable(
                name: "CriteriaForm");

            migrationBuilder.DropTable(
                name: "Criteria");

            migrationBuilder.DropTable(
                name: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Semester_CriteriaFormId",
                table: "Semester");

            migrationBuilder.DropIndex(
                name: "IX_IdeaRequest_CriteriaFormId",
                table: "IdeaRequest");

            migrationBuilder.DropColumn(
                name: "CriteriaFormId",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "NumberReviewer",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "CriteriaFormId",
                table: "IdeaRequest");

            migrationBuilder.RenameColumn(
                name: "TopicId",
                table: "Project",
                newName: "IdeaId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_TopicId",
                table: "Project",
                newName: "IX_Project_IdeaId");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "IdeaRequest",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdeaCode",
                table: "Idea",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdeaHistory",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    FileUpdate = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    ReviewStage = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaHistory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaHistory_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdeaHistory_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdeaHistoryRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    Content = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IdeaHistoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    ProcessDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaHistoryRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaHistoryRequest_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistory_IdeaId",
                table: "IdeaHistory",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistory_UserId",
                table: "IdeaHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaHistoryRequest_UserId",
                table: "IdeaHistoryRequest",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Idea_IdeaId",
                table: "Project",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}

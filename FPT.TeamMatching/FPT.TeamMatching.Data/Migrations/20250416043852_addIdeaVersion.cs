using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class addIdeaVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerCriteria_IdeaRequest_IdeaRequestId",
                table: "AnswerCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_Idea_StageIdea_StageIdeaId",
                table: "Idea");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Idea_IdeaId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicVersion_Idea_IdeaId",
                table: "TopicVersion");

            migrationBuilder.DropForeignKey(
                name: "FK_TopicVersion_User_UserId",
                table: "TopicVersion");

            migrationBuilder.DropTable(
                name: "IdeaRequest");

            migrationBuilder.DropTable(
                name: "MentorIdeaRequest");

            migrationBuilder.DropIndex(
                name: "IX_TopicVersion_IdeaId",
                table: "TopicVersion");

            migrationBuilder.DropIndex(
                name: "IX_TopicVersion_UserId",
                table: "TopicVersion");

            migrationBuilder.DropIndex(
                name: "IX_Idea_StageIdeaId",
                table: "Idea");

            migrationBuilder.DropColumn(
                name: "IdeaId",
                table: "TopicVersion");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TopicVersion");

            migrationBuilder.DropColumn(
                name: "NumberReviewer",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "StageIdeaId",
                table: "Idea");

            migrationBuilder.RenameColumn(
                name: "IdeaId",
                table: "Topic",
                newName: "IdeaVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Topic_IdeaId",
                table: "Topic",
                newName: "IX_Topic_IdeaVersionId");

            migrationBuilder.AddColumn<int>(
                name: "NumberReviewer",
                table: "StageIdea",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdeaVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    StageIdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaVersion_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdeaVersion_StageIdea_StageIdeaId",
                        column: x => x.StageIdeaId,
                        principalTable: "StageIdea",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MentorTopicRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorTopicRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorTopicRequest_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MentorTopicRequest_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TopicVersionRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TopicVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProcessDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicVersionRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicVersionRequest_TopicVersion_TopicVersionId",
                        column: x => x.TopicVersionId,
                        principalTable: "TopicVersion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TopicVersionRequest_User_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdeaVersionRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaVersionId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CriteriaFormId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
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
                    table.PrimaryKey("PK_IdeaVersionRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaVersionRequest_CriteriaForm_CriteriaFormId",
                        column: x => x.CriteriaFormId,
                        principalTable: "CriteriaForm",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdeaVersionRequest_IdeaVersion_IdeaVersionId",
                        column: x => x.IdeaVersionId,
                        principalTable: "IdeaVersion",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdeaVersionRequest_User_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_IdeaVersion_IdeaId",
                table: "IdeaVersion",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaVersion_StageIdeaId",
                table: "IdeaVersion",
                column: "StageIdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaVersionRequest_CriteriaFormId",
                table: "IdeaVersionRequest",
                column: "CriteriaFormId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaVersionRequest_IdeaVersionId",
                table: "IdeaVersionRequest",
                column: "IdeaVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaVersionRequest_ReviewerId",
                table: "IdeaVersionRequest",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorTopicRequest_ProjectId",
                table: "MentorTopicRequest",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorTopicRequest_TopicId",
                table: "MentorTopicRequest",
                column: "TopicId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicVersionRequest_ReviewerId",
                table: "TopicVersionRequest",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicVersionRequest_TopicVersionId",
                table: "TopicVersionRequest",
                column: "TopicVersionId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaRequestId",
                table: "AnswerCriteria",
                column: "IdeaRequestId",
                principalTable: "IdeaVersionRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_IdeaVersion_IdeaVersionId",
                table: "Topic",
                column: "IdeaVersionId",
                principalTable: "IdeaVersion",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaRequestId",
                table: "AnswerCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_IdeaVersion_IdeaVersionId",
                table: "Topic");

            migrationBuilder.DropTable(
                name: "IdeaVersionRequest");

            migrationBuilder.DropTable(
                name: "MentorTopicRequest");

            migrationBuilder.DropTable(
                name: "TopicVersionRequest");

            migrationBuilder.DropTable(
                name: "IdeaVersion");

            migrationBuilder.DropColumn(
                name: "NumberReviewer",
                table: "StageIdea");

            migrationBuilder.RenameColumn(
                name: "IdeaVersionId",
                table: "Topic",
                newName: "IdeaId");

            migrationBuilder.RenameIndex(
                name: "IX_Topic_IdeaVersionId",
                table: "Topic",
                newName: "IX_Topic_IdeaId");

            migrationBuilder.AddColumn<Guid>(
                name: "IdeaId",
                table: "TopicVersion",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "TopicVersion",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberReviewer",
                table: "Semester",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StageIdeaId",
                table: "Idea",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "IdeaRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CriteriaFormId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    ProcessDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Role = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaRequest_CriteriaForm_CriteriaFormId",
                        column: x => x.CriteriaFormId,
                        principalTable: "CriteriaForm",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_IdeaRequest_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdeaRequest_User_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MentorIdeaRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MentorIdeaRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorIdeaRequest_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MentorIdeaRequest_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TopicVersion_IdeaId",
                table: "TopicVersion",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicVersion_UserId",
                table: "TopicVersion",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Idea_StageIdeaId",
                table: "Idea",
                column: "StageIdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaRequest_CriteriaFormId",
                table: "IdeaRequest",
                column: "CriteriaFormId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaRequest_IdeaId",
                table: "IdeaRequest",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_IdeaRequest_ReviewerId",
                table: "IdeaRequest",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorIdeaRequest_IdeaId",
                table: "MentorIdeaRequest",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorIdeaRequest_ProjectId",
                table: "MentorIdeaRequest",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerCriteria_IdeaRequest_IdeaRequestId",
                table: "AnswerCriteria",
                column: "IdeaRequestId",
                principalTable: "IdeaRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Idea_StageIdea_StageIdeaId",
                table: "Idea",
                column: "StageIdeaId",
                principalTable: "StageIdea",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Idea_IdeaId",
                table: "Topic",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TopicVersion_Idea_IdeaId",
                table: "TopicVersion",
                column: "IdeaId",
                principalTable: "Idea",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TopicVersion_User_UserId",
                table: "TopicVersion",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}

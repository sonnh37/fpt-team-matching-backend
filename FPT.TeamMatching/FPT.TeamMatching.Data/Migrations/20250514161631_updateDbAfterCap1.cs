using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class updateDbAfterCap1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaVersionRequestId",
                table: "AnswerCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_User_UserId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_IdeaVersion_IdeaVersionId",
                table: "Topic");

            migrationBuilder.DropTable(
                name: "IdeaVersionRequest");

            migrationBuilder.DropTable(
                name: "Timeline");

            migrationBuilder.DropTable(
                name: "IdeaVersion");

            migrationBuilder.DropTable(
                name: "Idea");

            migrationBuilder.DropTable(
                name: "StageIdea");

            migrationBuilder.DropIndex(
                name: "IX_Topic_IdeaVersionId",
                table: "Topic");

            migrationBuilder.RenameColumn(
                name: "IdeaVersionId",
                table: "Topic",
                newName: "SubMentorId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Project",
                newName: "SemesterId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_UserId",
                table: "Project",
                newName: "IX_Project_SemesterId");

            migrationBuilder.RenameColumn(
                name: "IdeaVersionRequestId",
                table: "AnswerCriteria",
                newName: "TopicRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerCriteria_IdeaVersionRequestId",
                table: "AnswerCriteria",
                newName: "IX_AnswerCriteria_TopicRequestId");

            migrationBuilder.AddColumn<string>(
                name: "Abbreviation",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnglishName",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnterpriseName",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FileUrl",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnterpriseTopic",
                table: "Topic",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsExistedTeam",
                table: "Topic",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "MentorId",
                table: "Topic",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OwnerId",
                table: "Topic",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "SpecialtyId",
                table: "Topic",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "StageTopicId",
                table: "Topic",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VietNameseName",
                table: "Topic",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxTeamSize",
                table: "Semester",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MinTeamSize",
                table: "Semester",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfTeam",
                table: "Semester",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "OnGoingDate",
                table: "Semester",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Semester",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "StageTopic",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResultDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StageNumber = table.Column<int>(type: "integer", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageTopic", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageTopic_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TopicRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    TopicId = table.Column<Guid>(type: "uuid", nullable: true),
                    ReviewerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CriteriaFormId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    ProcessDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("PK_TopicRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TopicRequest_CriteriaForm_CriteriaFormId",
                        column: x => x.CriteriaFormId,
                        principalTable: "CriteriaForm",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TopicRequest_Topic_TopicId",
                        column: x => x.TopicId,
                        principalTable: "Topic",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TopicRequest_User_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topic_MentorId",
                table: "Topic",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_OwnerId",
                table: "Topic",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_SpecialtyId",
                table: "Topic",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_StageTopicId",
                table: "Topic",
                column: "StageTopicId");

            migrationBuilder.CreateIndex(
                name: "IX_Topic_SubMentorId",
                table: "Topic",
                column: "SubMentorId");

            migrationBuilder.CreateIndex(
                name: "IX_StageTopic_SemesterId",
                table: "StageTopic",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicRequest_CriteriaFormId",
                table: "TopicRequest",
                column: "CriteriaFormId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicRequest_ReviewerId",
                table: "TopicRequest",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_TopicRequest_TopicId",
                table: "TopicRequest",
                column: "TopicId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerCriteria_TopicRequest_TopicRequestId",
                table: "AnswerCriteria",
                column: "TopicRequestId",
                principalTable: "TopicRequest",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Semester_SemesterId",
                table: "Project",
                column: "SemesterId",
                principalTable: "Semester",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_Specialty_SpecialtyId",
                table: "Topic",
                column: "SpecialtyId",
                principalTable: "Specialty",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_StageTopic_StageTopicId",
                table: "Topic",
                column: "StageTopicId",
                principalTable: "StageTopic",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_User_MentorId",
                table: "Topic",
                column: "MentorId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_User_OwnerId",
                table: "Topic",
                column: "OwnerId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_User_SubMentorId",
                table: "Topic",
                column: "SubMentorId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnswerCriteria_TopicRequest_TopicRequestId",
                table: "AnswerCriteria");

            migrationBuilder.DropForeignKey(
                name: "FK_Project_Semester_SemesterId",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_Specialty_SpecialtyId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_StageTopic_StageTopicId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_User_MentorId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_User_OwnerId",
                table: "Topic");

            migrationBuilder.DropForeignKey(
                name: "FK_Topic_User_SubMentorId",
                table: "Topic");

            migrationBuilder.DropTable(
                name: "StageTopic");

            migrationBuilder.DropTable(
                name: "TopicRequest");

            migrationBuilder.DropIndex(
                name: "IX_Topic_MentorId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Topic_OwnerId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Topic_SpecialtyId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Topic_StageTopicId",
                table: "Topic");

            migrationBuilder.DropIndex(
                name: "IX_Topic_SubMentorId",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "Abbreviation",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "EnglishName",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "EnterpriseName",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "FileUrl",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "IsEnterpriseTopic",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "IsExistedTeam",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "MentorId",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "SpecialtyId",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "StageTopicId",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "VietNameseName",
                table: "Topic");

            migrationBuilder.DropColumn(
                name: "MaxTeamSize",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "MinTeamSize",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "NumberOfTeam",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "OnGoingDate",
                table: "Semester");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Semester");

            migrationBuilder.RenameColumn(
                name: "SubMentorId",
                table: "Topic",
                newName: "IdeaVersionId");

            migrationBuilder.RenameColumn(
                name: "SemesterId",
                table: "Project",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Project_SemesterId",
                table: "Project",
                newName: "IX_Project_UserId");

            migrationBuilder.RenameColumn(
                name: "TopicRequestId",
                table: "AnswerCriteria",
                newName: "IdeaVersionRequestId");

            migrationBuilder.RenameIndex(
                name: "IX_AnswerCriteria_TopicRequestId",
                table: "AnswerCriteria",
                newName: "IX_AnswerCriteria_IdeaVersionRequestId");

            migrationBuilder.CreateTable(
                name: "Idea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    MentorId = table.Column<Guid>(type: "uuid", nullable: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    SpecialtyId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubMentorId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    IsEnterpriseTopic = table.Column<bool>(type: "boolean", nullable: false),
                    IsExistedTeam = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Idea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Idea_Specialty_SpecialtyId",
                        column: x => x.SpecialtyId,
                        principalTable: "Specialty",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Idea_User_MentorId",
                        column: x => x.MentorId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Idea_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Idea_User_SubMentorId",
                        column: x => x.SubMentorId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StageIdea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    NumberReviewer = table.Column<int>(type: "integer", nullable: true),
                    ResultDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    StageNumber = table.Column<int>(type: "integer", nullable: false),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StageIdea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StageIdea_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Timeline",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Title = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Timeline", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Timeline_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdeaVersion",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    StageIdeaId = table.Column<Guid>(type: "uuid", nullable: true),
                    Abbreviations = table.Column<string>(type: "text", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    EnglishName = table.Column<string>(type: "text", nullable: true),
                    EnterpriseName = table.Column<string>(type: "text", nullable: true),
                    File = table.Column<string>(type: "text", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true),
                    TeamSize = table.Column<int>(type: "integer", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    Version = table.Column<int>(type: "integer", nullable: true),
                    VietNamName = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdeaVersion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdeaVersion_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdeaVersion_StageIdea_StageIdeaId",
                        column: x => x.StageIdeaId,
                        principalTable: "StageIdea",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "IdeaVersionRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    CriteriaFormId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdeaVersionId = table.Column<Guid>(type: "uuid", nullable: true),
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
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdeaVersionRequest_User_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Topic_IdeaVersionId",
                table: "Topic",
                column: "IdeaVersionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Idea_MentorId",
                table: "Idea",
                column: "MentorId");

            migrationBuilder.CreateIndex(
                name: "IX_Idea_OwnerId",
                table: "Idea",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Idea_SpecialtyId",
                table: "Idea",
                column: "SpecialtyId");

            migrationBuilder.CreateIndex(
                name: "IX_Idea_SubMentorId",
                table: "Idea",
                column: "SubMentorId");

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
                name: "IX_StageIdea_SemesterId",
                table: "StageIdea",
                column: "SemesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Timeline_SemesterId",
                table: "Timeline",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_AnswerCriteria_IdeaVersionRequest_IdeaVersionRequestId",
                table: "AnswerCriteria",
                column: "IdeaVersionRequestId",
                principalTable: "IdeaVersionRequest",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_User_UserId",
                table: "Project",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Topic_IdeaVersion_IdeaVersionId",
                table: "Topic",
                column: "IdeaVersionId",
                principalTable: "IdeaVersion",
                principalColumn: "Id");
        }
    }
}

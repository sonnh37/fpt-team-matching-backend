using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCapstoneScheduleAndMentorIdeaRequestAndStageIdea : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Reviewer1",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Feedback");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Feedback");

            migrationBuilder.RenameColumn(
                name: "Reviewer2",
                table: "Review",
                newName: "Room");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Project",
                newName: "TeamCode");

            migrationBuilder.RenameColumn(
                name: "FileUpload",
                table: "Feedback",
                newName: "Comment");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpirationDate",
                table: "Review",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ReviewDate",
                table: "Review",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Reviewer1Id",
                table: "Review",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Reviewer2Id",
                table: "Review",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Slot",
                table: "Review",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Rate",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Profession",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "CapstoneSchedule",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    Time = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    HallName = table.Column<string>(type: "text", nullable: true),
                    Stage = table.Column<int>(type: "integer", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapstoneSchedule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CapstoneSchedule_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "MentorIdeaRequest",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    ProjectId = table.Column<Guid>(type: "uuid", nullable: true),
                    IdeaId = table.Column<Guid>(type: "uuid", nullable: true),
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
                    table.PrimaryKey("PK_MentorIdeaRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MentorIdeaRequest_Idea_IdeaId",
                        column: x => x.IdeaId,
                        principalTable: "Idea",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MentorIdeaRequest_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StageIdea",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: true),
                    StartDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    ResultDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
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

            migrationBuilder.CreateIndex(
                name: "IX_Review_Reviewer1Id",
                table: "Review",
                column: "Reviewer1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Review_Reviewer2Id",
                table: "Review",
                column: "Reviewer2Id");

            migrationBuilder.CreateIndex(
                name: "IX_CapstoneSchedule_ProjectId",
                table: "CapstoneSchedule",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorIdeaRequest_IdeaId",
                table: "MentorIdeaRequest",
                column: "IdeaId");

            migrationBuilder.CreateIndex(
                name: "IX_MentorIdeaRequest_ProjectId",
                table: "MentorIdeaRequest",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StageIdea_SemesterId",
                table: "StageIdea",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_User_Reviewer1Id",
                table: "Review",
                column: "Reviewer1Id",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_User_Reviewer2Id",
                table: "Review",
                column: "Reviewer2Id",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_User_Reviewer1Id",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_User_Reviewer2Id",
                table: "Review");

            migrationBuilder.DropTable(
                name: "CapstoneSchedule");

            migrationBuilder.DropTable(
                name: "MentorIdeaRequest");

            migrationBuilder.DropTable(
                name: "StageIdea");

            migrationBuilder.DropIndex(
                name: "IX_Review_Reviewer1Id",
                table: "Review");

            migrationBuilder.DropIndex(
                name: "IX_Review_Reviewer2Id",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ReviewDate",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Reviewer1Id",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Reviewer2Id",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Slot",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Content",
                table: "Rate");

            migrationBuilder.RenameColumn(
                name: "Room",
                table: "Review",
                newName: "Reviewer2");

            migrationBuilder.RenameColumn(
                name: "TeamCode",
                table: "Project",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Feedback",
                newName: "FileUpload");

            migrationBuilder.AddColumn<string>(
                name: "Reviewer1",
                table: "Review",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Project",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "EndDate",
                table: "Project",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "StartDate",
                table: "Project",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Profession",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Feedback",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Feedback",
                type: "text",
                nullable: true);
        }
    }
}

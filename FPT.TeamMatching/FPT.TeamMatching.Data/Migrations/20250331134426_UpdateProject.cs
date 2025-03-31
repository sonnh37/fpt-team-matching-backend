using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProject : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_Project_ProjectId",
                table: "Blog");

            migrationBuilder.DropForeignKey(
                name: "FK_CapstoneSchedule_Project_ProjectId",
                table: "CapstoneSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_Project_ProjectId",
                table: "Invitation");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorIdeaRequest_Project_ProjectId",
                table: "MentorIdeaRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Project_ProjectId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Project_ProjectId",
                table: "TeamMember");

            migrationBuilder.AddColumn<int>(
                name: "DefenseStage",
                table: "Project",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_Project_ProjectId",
                table: "Blog",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CapstoneSchedule_Project_ProjectId",
                table: "CapstoneSchedule",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_Project_ProjectId",
                table: "Invitation",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MentorIdeaRequest_Project_ProjectId",
                table: "MentorIdeaRequest",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Project_ProjectId",
                table: "Review",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Project_ProjectId",
                table: "TeamMember",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blog_Project_ProjectId",
                table: "Blog");

            migrationBuilder.DropForeignKey(
                name: "FK_CapstoneSchedule_Project_ProjectId",
                table: "CapstoneSchedule");

            migrationBuilder.DropForeignKey(
                name: "FK_Invitation_Project_ProjectId",
                table: "Invitation");

            migrationBuilder.DropForeignKey(
                name: "FK_MentorIdeaRequest_Project_ProjectId",
                table: "MentorIdeaRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_Review_Project_ProjectId",
                table: "Review");

            migrationBuilder.DropForeignKey(
                name: "FK_TeamMember_Project_ProjectId",
                table: "TeamMember");

            migrationBuilder.DropColumn(
                name: "DefenseStage",
                table: "Project");

            migrationBuilder.AddForeignKey(
                name: "FK_Blog_Project_ProjectId",
                table: "Blog",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CapstoneSchedule_Project_ProjectId",
                table: "CapstoneSchedule",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitation_Project_ProjectId",
                table: "Invitation",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MentorIdeaRequest_Project_ProjectId",
                table: "MentorIdeaRequest",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_Project_ProjectId",
                table: "Review",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMember_Project_ProjectId",
                table: "TeamMember",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id");
        }
    }
}

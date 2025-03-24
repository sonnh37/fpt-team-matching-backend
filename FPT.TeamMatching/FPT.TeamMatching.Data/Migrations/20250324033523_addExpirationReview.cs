using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class addExpirationReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDate",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Review");

            migrationBuilder.AddColumn<Guid>(
                name: "ExpirationReviewId",
                table: "Review",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExpirationReview",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false, defaultValueSql: "gen_random_uuid()"),
                    SemesterId = table.Column<Guid>(type: "uuid", nullable: true),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    ExpirationDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    CreatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedDate = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                    Note = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExpirationReview", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExpirationReview_Semester_SemesterId",
                        column: x => x.SemesterId,
                        principalTable: "Semester",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Review_ExpirationReviewId",
                table: "Review",
                column: "ExpirationReviewId");

            migrationBuilder.CreateIndex(
                name: "IX_ExpirationReview_SemesterId",
                table: "ExpirationReview",
                column: "SemesterId");

            migrationBuilder.AddForeignKey(
                name: "FK_Review_ExpirationReview_ExpirationReviewId",
                table: "Review",
                column: "ExpirationReviewId",
                principalTable: "ExpirationReview",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Review_ExpirationReview_ExpirationReviewId",
                table: "Review");

            migrationBuilder.DropTable(
                name: "ExpirationReview");

            migrationBuilder.DropIndex(
                name: "IX_Review_ExpirationReviewId",
                table: "Review");

            migrationBuilder.DropColumn(
                name: "ExpirationReviewId",
                table: "Review");

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "ExpirationDate",
                table: "Review",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Review",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}

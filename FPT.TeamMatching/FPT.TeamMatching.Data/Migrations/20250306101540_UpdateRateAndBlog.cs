using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FPT.TeamMatching.Data.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRateAndBlog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rate_TeamMember_TeamMemberId",
                table: "Rate");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_User_RateById",
                table: "Rate");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_User_RateForId",
                table: "Rate");

            migrationBuilder.DropIndex(
                name: "IX_Rate_TeamMemberId",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "TeamMemberId",
                table: "Rate");

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Blog",
                type: "text",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_TeamMember_RateById",
                table: "Rate",
                column: "RateById",
                principalTable: "TeamMember",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_TeamMember_RateForId",
                table: "Rate",
                column: "RateForId",
                principalTable: "TeamMember",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rate_TeamMember_RateById",
                table: "Rate");

            migrationBuilder.DropForeignKey(
                name: "FK_Rate_TeamMember_RateForId",
                table: "Rate");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Blog");

            migrationBuilder.AddColumn<Guid>(
                name: "TeamMemberId",
                table: "Rate",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Rate_TeamMemberId",
                table: "Rate",
                column: "TeamMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_TeamMember_TeamMemberId",
                table: "Rate",
                column: "TeamMemberId",
                principalTable: "TeamMember",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_User_RateById",
                table: "Rate",
                column: "RateById",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rate_User_RateForId",
                table: "Rate",
                column: "RateForId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}

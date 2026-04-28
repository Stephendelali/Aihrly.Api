using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aihrly.Api.Migrations
{
    /// <inheritdoc />
    public partial class RenameScoreFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ScoredById",
                table: "Scores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_StageHistories_ChangedById",
                table: "StageHistories",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_Scores_ScoredById",
                table: "Scores",
                column: "ScoredById");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationNotes_CreatedById",
                table: "ApplicationNotes",
                column: "CreatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicationNotes_TeamMembers_CreatedById",
                table: "ApplicationNotes",
                column: "CreatedById",
                principalTable: "TeamMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Scores_TeamMembers_ScoredById",
                table: "Scores",
                column: "ScoredById",
                principalTable: "TeamMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StageHistories_TeamMembers_ChangedById",
                table: "StageHistories",
                column: "ChangedById",
                principalTable: "TeamMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicationNotes_TeamMembers_CreatedById",
                table: "ApplicationNotes");

            migrationBuilder.DropForeignKey(
                name: "FK_Scores_TeamMembers_ScoredById",
                table: "Scores");

            migrationBuilder.DropForeignKey(
                name: "FK_StageHistories_TeamMembers_ChangedById",
                table: "StageHistories");

            migrationBuilder.DropIndex(
                name: "IX_StageHistories_ChangedById",
                table: "StageHistories");

            migrationBuilder.DropIndex(
                name: "IX_Scores_ScoredById",
                table: "Scores");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationNotes_CreatedById",
                table: "ApplicationNotes");

            migrationBuilder.DropColumn(
                name: "ScoredById",
                table: "Scores");
        }
    }
}

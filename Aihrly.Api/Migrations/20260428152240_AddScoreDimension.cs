using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Aihrly.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddScoreDimension : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Scores");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Scores",
                newName: "ScoredAt");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Scores",
                newName: "Dimension");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Scores",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ScoredAt",
                table: "Scores",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "Dimension",
                table: "Scores",
                newName: "Type");

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Scores",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UpdatedById",
                table: "Scores",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}

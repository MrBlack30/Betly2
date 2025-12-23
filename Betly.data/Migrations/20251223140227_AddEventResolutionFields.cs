using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Betly.data.Migrations
{
    /// <inheritdoc />
    public partial class AddEventResolutionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bets_Users_UserId",
                table: "Bets");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Bets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EventId",
                table: "Bets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Outcome",
                table: "Bets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SelectedOutcome",
                table: "Bets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TeamA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TeamB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OddsTeamA = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OddsTeamB = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OddsDraw = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsResolved = table.Column<bool>(type: "bit", nullable: false),
                    Winner = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bets_EventId",
                table: "Bets",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bets_Events_EventId",
                table: "Bets",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bets_Users_UserId",
                table: "Bets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bets_Events_EventId",
                table: "Bets");

            migrationBuilder.DropForeignKey(
                name: "FK_Bets_Users_UserId",
                table: "Bets");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Bets_EventId",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "Outcome",
                table: "Bets");

            migrationBuilder.DropColumn(
                name: "SelectedOutcome",
                table: "Bets");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Bets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Bets_Users_UserId",
                table: "Bets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}

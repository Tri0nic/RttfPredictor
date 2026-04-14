using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactApp1.Server.Migrations
{
    /// <inheritdoc />
    public partial class TournamentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE player_stats ALTER COLUMN tournament_id TYPE bigint USING tournament_id::bigint;");
            migrationBuilder.Sql("ALTER TABLE player_stats ALTER COLUMN player_id TYPE bigint USING player_id::bigint;");

            migrationBuilder.CreateTable(
                name: "tournaments",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false),
                    starts_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tournaments", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_player_stats_tournament_id",
                table: "player_stats",
                column: "tournament_id");

            migrationBuilder.AddForeignKey(
                name: "FK_player_stats_tournaments_tournament_id",
                table: "player_stats",
                column: "tournament_id",
                principalTable: "tournaments",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_player_stats_tournaments_tournament_id",
                table: "player_stats");

            migrationBuilder.DropTable(
                name: "tournaments");

            migrationBuilder.DropIndex(
                name: "IX_player_stats_tournament_id",
                table: "player_stats");

            migrationBuilder.AlterColumn<string>(
                name: "tournament_id",
                table: "player_stats",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<string>(
                name: "player_id",
                table: "player_stats",
                type: "text",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}

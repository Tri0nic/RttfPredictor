using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactApp1.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComparePredictionsViewOrdering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW compare_predictions AS
                SELECT
                    ps.player_id,
                    ps.tournament_id,
                    ps.position,
                    p.predicted_position,
                    t.starts_at
                FROM player_stats ps
                JOIN predictions p ON p.player_id = ps.player_id
                    AND p.tournament_id = ps.tournament_id
                JOIN tournaments t ON t.id = ps.tournament_id
                ORDER BY ps.position ASC, p.predicted_position ASC
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                CREATE OR REPLACE VIEW compare_predictions AS
                SELECT
                    ps.player_id,
                    ps.tournament_id,
                    ps.position,
                    p.predicted_position,
                    t.starts_at
                FROM player_stats ps
                JOIN predictions p ON p.player_id = ps.player_id
                    AND p.tournament_id = ps.tournament_id
                JOIN tournaments t ON t.id = ps.tournament_id
            ");
        }
    }
}

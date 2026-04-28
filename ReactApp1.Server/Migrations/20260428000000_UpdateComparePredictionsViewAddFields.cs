using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReactApp1.Server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateComparePredictionsViewAddFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS compare_predictions;");
            migrationBuilder.Sql(@"
                CREATE VIEW compare_predictions AS
                SELECT
                    ps.tournament_id,
                    ps.player_id,
                    ps.name,
                    ps.rating,
                    ps.position,
                    p.predicted_position,
                    ps.won_games,
                    ps.lost_games,
                    CASE
                        WHEN (ps.won_games + ps.lost_games) > 0
                        THEN ROUND(ps.won_games::numeric / (ps.won_games + ps.lost_games), 3)
                        ELSE NULL
                    END AS winrate,
                    t.starts_at
                FROM player_stats ps
                JOIN predictions p ON p.player_id = ps.player_id
                    AND p.tournament_id = ps.tournament_id
                JOIN tournaments t ON t.id = ps.tournament_id
                ORDER BY ps.tournament_id DESC, ps.position ASC, p.predicted_position ASC
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"DROP VIEW IF EXISTS compare_predictions;");
            migrationBuilder.Sql(@"
                CREATE VIEW compare_predictions AS
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
    }
}

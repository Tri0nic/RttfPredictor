using System.Text.Json.Serialization;

namespace ReactApp1.Server.DTO
{
    public class PythonPredictRequest
    {
        [JsonPropertyName("player_id")]
        public long PlayerId { get; set; }

        [JsonPropertyName("tournament_id")]
        public long TournamentId { get; set; }

        [JsonPropertyName("rating")]
        public int? Rating { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("arm")]
        public string? Arm { get; set; }

        [JsonPropertyName("tournaments_played")]
        public int? TournamentsPlayed { get; set; }

        [JsonPropertyName("won_games")]
        public int? WonGames { get; set; }

        [JsonPropertyName("lost_games")]
        public int? LostGames { get; set; }

        [JsonPropertyName("avg_tournament_rating")]
        public double? AvgTournamentRating { get; set; }
    }
}

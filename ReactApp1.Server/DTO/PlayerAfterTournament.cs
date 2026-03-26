using System.Text.Json.Serialization;

namespace ReactApp1.Server.DTO
{
    public class PlayerAfterTournament
    {
        [JsonPropertyName("link")]
        public required string Link { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("rating_before")]
        public int RatingBefore { get; set; }

        [JsonPropertyName("rating_delta")]
        public decimal RatingDelta { get; set; }

        [JsonPropertyName("rating_after")]
        public int RatingAfter { get; set; }

        [JsonPropertyName("games")]
        public int Games { get; set; }

        [JsonPropertyName("games_won")]
        public int GamesWon { get; set; }

        [JsonPropertyName("games_lost")]
        public int GamesLost { get; set; }

        [JsonPropertyName("sets")]
        public int Sets { get; set; }

        [JsonPropertyName("sets_won")]
        public int SetsWon { get; set; }

        [JsonPropertyName("sets_lost")]
        public int SetsLost { get; set; }
    }
}

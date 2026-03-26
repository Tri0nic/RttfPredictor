using System.Text.Json.Serialization;

namespace ReactApp1.Server.DTO
{
    public class PlayerStats
    {
        [JsonPropertyName("link")]
        public required string Link { get; set; }

        [JsonPropertyName("name")]
        public required string Name { get; set; }


        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("year")]
        public int? Year { get; set; }

        [JsonPropertyName("arm")]
        public string? Arm { get; set; }

        [JsonPropertyName("rating")]
        public int Rating { get; set; }

        [JsonPropertyName("tournaments_played")]
        public int TournamentsPlayed { get; set; }

        [JsonPropertyName("won_games")]
        public int WonGames { get; set; }

        [JsonPropertyName("lost_games")]
        public int LostGames { get; set; }
    }
}

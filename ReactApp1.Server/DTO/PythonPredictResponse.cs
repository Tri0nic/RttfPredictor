using System.Text.Json.Serialization;

namespace ReactApp1.Server.DTO
{
    public class PythonPredictResponse
    {
        [JsonPropertyName("player_id")]
        public long PlayerId { get; set; }

        [JsonPropertyName("tournament_id")]
        public long TournamentId { get; set; }

        [JsonPropertyName("predicted_position")]
        public int PredictedPosition { get; set; }

        [JsonPropertyName("prediction_score")]
        public double PredictionScore { get; set; }

        [JsonPropertyName("model_version")]
        public string ModelVersion { get; set; } = "";

        [JsonPropertyName("predicted_at")]
        public DateTime PredictedAt { get; set; }
    }
}

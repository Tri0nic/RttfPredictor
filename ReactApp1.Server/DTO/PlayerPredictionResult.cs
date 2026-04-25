namespace ReactApp1.Server.DTO
{
    public class PlayerPredictionResult
    {
        public long PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public long TournamentId { get; set; }
        public int PredictedPosition { get; set; }
        public DateTime PredictedAt { get; set; }
    }
}

namespace ReactApp1.Server.DTO
{
    public class TournamentPredictionItem
    {
        public long PlayerId { get; set; }
        public string? PlayerName { get; set; }
        public int? Rating { get; set; }
        public int PredictedPosition { get; set; }
        public DateTime PredictedAt { get; set; }
    }
}
